const express = require('express');
const cors = require('cors');
const bcrypt = require('bcrypt');
const jwt = require('jsonwebtoken');
const Database = require('better-sqlite3');

const app = express();
const db = new Database('chasse.db');
const JWT_SECRET = 'change_this_secret_in_production_student_project';

app.use(cors());
app.use(express.json());

// Création table
db.exec(`
  CREATE TABLE IF NOT EXISTS users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT UNIQUE NOT NULL,
    email TEXT UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    created_at TEXT DEFAULT CURRENT_TIMESTAMP
  )
`);

app.post('/api/register', async (req, res) => {
  try {
    const { username, email, password } = req.body;

    if (!username || !email || !password) {
      return res.json({ success: false, message: 'All fields are required' });
    }

    if (password.length < 6) {
      return res.json({ success: false, message: 'Password too short' });
    }

    const hash = await bcrypt.hash(password, 10);

    const stmt = db.prepare('INSERT INTO users (username, email, password_hash) VALUES (?, ?, ?)');
    stmt.run(username, email, hash);

    const token = jwt.sign({ username }, JWT_SECRET, { expiresIn: '7d' });

    res.json({
      success: true,
      message: 'Account created',
      token,
      username
    });
  } catch (err) {
    if (err.code === 'SQLITE_CONSTRAINT_UNIQUE') {
      return res.json({ success: false, message: 'Username or email already exists' });
    }
    console.error(err);
    res.json({ success: false, message: 'Server error' });
  }
});

app.post('/api/login', async (req, res) => {
  try {
    const { login, password } = req.body;

    if (!login || !password) {
      return res.json({ success: false, message: 'All fields are required' });
    }

    // On cherche par username OU email
    const user = db.prepare(
      'SELECT * FROM users WHERE username = ? OR email = ?'
    ).get(login, login);

    if (!user) {
      // Message volontairement vague
      return res.json({ success: false, message: 'Wrong username or password' });
    }

    const match = await bcrypt.compare(password, user.password_hash);

    if (!match) {
      return res.json({ success: false, message: 'Wrong username or password' });
    }

    const token = jwt.sign({ username: user.username }, JWT_SECRET, { expiresIn: '7d' });

    res.json({
      success: true,
      message: 'Login successful',
      token,
      username: user.username
    });
  } catch (err) {
    console.error(err);
    res.json({ success: false, message: 'Server error' });
  }
});

app.listen(3000, () => {
  console.log('Backend running on http://localhost:3000');
});
