#!/bin/bash
# Push to GitHub - SafeVault Security Project

echo "========================================="
echo "Pushing SafeVault Security to GitHub"
echo "========================================="

# Configuration
REPO_URL="https://github.com/MootezMnasri/Security_Feature.git"
BRANCH="main"

# Check if git is initialized
if [ ! -d ".git" ]; then
    echo "[INFO] Initializing git repository..."
    git init
    git remote add origin $REPO_URL
fi

# Set branch
git checkout -B $BRANCH 2>/dev/null || git branch -M $BRANCH

# Add all files (except node_modules, .git, etc.)
echo "[INFO] Staging files..."
git add .gitignore README.md database.sql server.js package.json package-lock.json
git add Tests/
git add src/
git add webform.html 2>/dev/null || true

# Check if there are changes to commit
if git diff --cached --quiet; then
    echo "[INFO] No new changes to commit."
else
    # Commit changes
    echo "[INFO] Committing changes..."
    git commit -m "feat(security): implement secure input validation, authentication, and RBAC

- Add input validation with SQL injection and XSS prevention
- Implement parameterized queries for database operations
- Add bcrypt password hashing and JWT authentication
- Implement role-based authorization (Admin/User)
- Add comprehensive security tests
- Document vulnerabilities identified and fixes applied"
fi

# Pull and push
echo "[INFO] Pulling latest changes..."
git pull --rebase origin $BRANCH 2>/dev/null || echo "[INFO] No remote changes to pull"

echo "[INFO] Pushing to GitHub..."
git push -u origin $BRANCH

echo "========================================="
echo "Done! Repository pushed to:"
echo $REPO_URL
echo "========================================="
