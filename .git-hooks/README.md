# Git Hooks for SwordNStone

This directory contains Git hooks that help prevent build errors and maintain code quality.

## Available Hooks

### pre-commit

Validates files before allowing a commit. Checks for:
- .ci.cs files included in .csproj
- Well-formed XML in .csproj files
- Common CiTo syntax issues
- Modified dependencies

## Installation

**Linux/Mac:**
```bash
cp .git-hooks/pre-commit .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit
```

**Windows (Git Bash):**
```cmd
copy .git-hooks\pre-commit .git\hooks\pre-commit
```

**Windows (PowerShell):**
```powershell
Copy-Item .git-hooks\pre-commit .git\hooks\pre-commit
```

## Testing the Hook

After installation, try making a commit. The hook will automatically run validation.

To bypass the hook temporarily (not recommended):
```bash
git commit --no-verify
```

## Customization

Edit the hook files in `.git-hooks/` and reinstall to customize behavior.

## Why Use Hooks?

Git hooks automate validation and prevent common mistakes:
- ✅ Catches missing project references before commit
- ✅ Validates XML file structure
- ✅ Warns about CiTo syntax issues
- ✅ Reminds to run full validation
- ✅ Prevents broken commits from entering the repository

This reduces CI failures and makes code reviews smoother.
