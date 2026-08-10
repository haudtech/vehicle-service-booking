# Coding Convention Guide

This document defines the standard coding conventions for the Vehicle Service Booking repository.

## Purpose

Keep C# code consistent across the solution and make formatting rules easy to find.

## Repository-wide configuration

- `.editorconfig` defines editor and formatting rules such as indentation, newline style, and C# style preferences.
- `Directory.Build.props` applies shared MSBuild settings, including analyzer enablement and build-time code-style checks.

## How to use it

- Use the workspace `.editorconfig` settings for formatting and code style.
- Use the shared `Directory.Build.props` settings to enable analyzers and enforcement in every project.
- Do not duplicate the same analyzer properties in individual project files unless a project needs a specific override.

## Recommended workflow

1. Open a C# file in the workspace.
2. Save the file to trigger formatter and style enforcement if editor settings are configured.
3. Use code cleanup or `Format Document` when necessary.
4. Keep the shared configuration centralized so every project inherits the same rules.
