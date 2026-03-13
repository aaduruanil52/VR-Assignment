const express = require('express');
const jwt = require('jsonwebtoken');
const cors = require('cors');

const app = express();

app.use(cors({
  origin: '*',
  methods: ['GET', 'POST', 'PUT', 'DELETE', 'OPTIONS'],
  allowedHeaders: ['Content-Type', 'Authorization', 'Accept'],
  credentials: false
}));

app.options('*', cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

const JWT_SECRET = 'visnet-xr-secret-key';

const USERS = [
  { id: 1, username: 'testuser', password: '123456', name: 'Test User' },
  { id: 2, username: 'admin', password: 'admin123', name: 'Admin User' },
];

const PROJECTS = [
  { id: 1, name: 'Project A' },
  { id: 2, name: 'Project B' },
  { id: 3, name: 'Project C' },
  { id: 4, name: 'Project D' },
];

const FLOORS = {
  1: ['Floor 1', 'Floor 2', 'Floor 3', 'Floor 4'],
  2: ['Floor A', 'Floor B'],
  3: ['Ground', '1', '2', '3', '4', '5'],
  4: ['Basement', 'Ground', 'Mezzanine'],
};

app.get('/', (req, res) => {
  res.json({ status: 'ViSNET XR API running' });
});

app.post('/api/login', (req, res) => {
  console.log('Login request body:', req.body);
  const { username, password } = req.body;

  if (!username || !password) {
    return res.status(400).json({ success: false, message: 'Username and password required' });
  }

  const user = USERS.find(u => u.username === username && u.password === password);

  if (!user) {
    return res.status(401).json({ success: false, message: 'Invalid credentials' });
  }

  const token = jwt.sign({ id: user.id, username: user.username }, JWT_SECRET, { expiresIn: '24h' });

  res.json({ success: true, token, user: { id: user.id, name: user.name } });
});

app.get('/api/projects', (req, res) => {
  res.json({ projects: PROJECTS });
});

app.get('/api/projects/:id/floors', (req, res) => {
  const projectId = parseInt(req.params.id);
  const floors = FLOORS[projectId];

  if (!floors) {
    return res.status(404).json({ success: false, message: 'Project not found' });
  }

  res.json({ floors });
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log('ViSNET XR API running on port ' + PORT));

module.exports = app;