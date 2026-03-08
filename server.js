// server.js
const express = require('express');
const mysql = require('mysql2');
const app = express();
app.use(express.urlencoded({ extended: true }));

// Input validation and sanitization
function sanitizeInput(input) {
    // Remove script tags and encode special characters
    return input.replace(/<.*?>/g, '')
                .replace(/["'`;]/g, '')
                .replace(/&/g, '&amp;')
                .replace(/</g, '&lt;')
                .replace(/>/g, '&gt;');
}

const db = mysql.createConnection({
    host: 'localhost',
    user: 'root',
    password: '',
    database: 'safevault'
});

app.post('/submit', (req, res) => {
    const username = sanitizeInput(req.body.username);
    const email = sanitizeInput(req.body.email);
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

app.listen(3000, () => {
    console.log('Server running on port 3000');
});
