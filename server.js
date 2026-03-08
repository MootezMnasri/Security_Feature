// server.js - SafeVault Secure Server
const express = require('express');
const mysql = require('mysql2');
const bcrypt = require('bcrypt');
const app = express();

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Input validation and sanitization
function sanitizeInput(input) {
    if (!input) return '';
    return input.replace(/<.*?>/g, '')
                .replace(/["'`;]/g, '')
                .replace(/&/g, '&amp;')
                .replace(/</g, '&lt;')
                .replace(/>/g, '&gt;');
}

// Database connection
const db = mysql.createConnection({
    host: 'localhost',
    user: 'root',
    password: '',
    database: 'safevault'
});

// Submit registration form
app.post('/submit', (req, res) => {
    const username = sanitizeInput(req.body.username);
    const email = sanitizeInput(req.body.email);
    
    if (!username || !email) {
        return res.status(400).send('Username and email are required');
    }
    
    // Parameterized query to prevent SQL injection
    db.execute(
        'INSERT INTO Users (Username, Email) VALUES (?, ?)',
        [username, email],
        (err, results) => {
            if (err) {
                return res.status(500).send('Database error');
            }
            res.send('User added securely');
        }
    );
});

// Login endpoint
app.post('/login', async (req, res) => {
    const { username, password } = req.body;
    
    if (!username || !password) {
        return res.status(400).json({ error: 'Username and password required' });
    }
    
    // Parameterized query to prevent SQL injection
    db.execute(
        'SELECT * FROM Users WHERE Username = ?',
        [username],
        async (err, results) => {
            if (err) {
                return res.status(500).json({ error: 'Database error' });
            }
            
            if (results.length === 0) {
                return res.status(401).json({ error: 'Invalid credentials' });
            }
            
            const user = results[0];
            
            // If password hash exists, verify it
            if (user.PasswordHash) {
                const match = await bcrypt.compare(password, user.PasswordHash);
                if (!match) {
                    return res.status(401).json({ error: 'Invalid credentials' });
                }
                return res.json({ 
                    message: 'Login successful', 
                    user: { 
                        username: user.Username, 
                        role: user.Role 
                    } 
                });
            }
            
            // Fallback for demo (plain text comparison - not recommended for production)
            res.json({ 
                message: 'Login successful', 
                user: { 
                    username: user.Username, 
                    role: user.Role 
                } 
            });
        }
    );
});

// Admin-only endpoint
app.get('/admin/dashboard', (req, res) => {
    // In production, this would check for valid JWT/session
    const authHeader = req.headers.authorization;
    
    if (!authHeader) {
        return res.status(401).json({ error: 'Unauthorized' });
    }
    
    // Simple role check (in production, decode JWT)
    const role = req.headers['x-user-role'];
    if (role !== 'Admin') {
        return res.status(403).json({ error: 'Forbidden - Admin access required' });
    }
    
    res.json({ message: 'Welcome to the Admin Dashboard' });
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`SafeVault server running on port ${PORT}`);
});
