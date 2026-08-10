# Azurite Terminal Guide

## Purpose

This guide documents how to install, configure, start, verify, stop, and reset Azurite for local development in this repository.

Azurite is used here as the local Azure Storage emulator for:
- Blob service on `127.0.0.1:10000`
- Queue service on `127.0.0.1:10001`
- Table service on `127.0.0.1:10002`

This is especially relevant for the Notification Functions local workflow.

---

## 1. Install Azurite

### Option A: global install with npm

```bash
npm install -g azurite
```

### Option B: run without global install

```bash
npx azurite --help
```

### Verify install

```bash
command -v azurite
azurite --version
```

Expected result:
- `command -v azurite` prints a binary path such as `/opt/homebrew/bin/azurite`

---

## 2. Local Configuration

Recommended repository-local storage path:

```bash
.azurite/
```

Recommended start command from repository root:

```bash
mkdir -p .azurite
azurite --location .azurite --debug .azurite/debug.log
```

What this does:
- stores Azurite data files under `.azurite`
- writes debug logs to `.azurite/debug.log`
- uses default local ports `10000`, `10001`, and `10002`

Default local endpoints:

```text
Blob  : http://127.0.0.1:10000
Queue : http://127.0.0.1:10001
Table : http://127.0.0.1:10002
```

Standard Azurite connection string:

```text
UseDevelopmentStorage=true
```

Equivalent explicit connection string:

```text
DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;
```

---

## 3. Start Azurite

From repository root:

```bash
cd /Users/tech/dev/net/vehicle-service-booking
mkdir -p .azurite
azurite --location .azurite --debug .azurite/debug.log
```

Expected startup lines:

```text
Azurite Blob service is successfully listening at http://127.0.0.1:10000
Azurite Queue service is successfully listening at http://127.0.0.1:10001
Azurite Table service is successfully listening at http://127.0.0.1:10002
```

If you need Azurite to keep running while you continue other work, start it in a dedicated terminal session.

---

## 4. Check Whether Azurite Is Running

### Check binary

```bash
command -v azurite
```

### Check listening ports

```bash
lsof -n -P -iTCP -sTCP:LISTEN | grep -E ':(10000|10001|10002)\b'
```

Expected result when running:
- listeners on ports `10000`, `10001`, and `10002`

### Check live debug log

```bash
tail -f .azurite/debug.log
```

### Check from HTTP traffic

When the Notification Functions host is connected, Azurite log lines will show requests against paths like:

```text
/devstoreaccount1/user-notification-events
/devstoreaccount1/user-notification-events-poison
```

---

## 5. Expected Behavior With Notification Functions

When the Functions host starts before the queues exist, Azurite may log repeated `404` requests like:

```text
GET /devstoreaccount1/user-notification-events?comp=metadata HTTP/1.1 404
GET /devstoreaccount1/user-notification-events-poison?comp=metadata HTTP/1.1 404
```

This is expected during local startup.

Once the queue is created or a message is published, the log should move to `200`, `201`, or `204` responses, for example:

```text
PUT /devstoreaccount1/user-notification-events HTTP/1.1 201
POST /devstoreaccount1/user-notification-events/messages HTTP/1.1 201
GET /devstoreaccount1/user-notification-events/messages?... HTTP/1.1 200
```

---

## 6. Stop Azurite

### If running in the current foreground terminal

Press:

```text
Ctrl+C
```

### If running in another terminal session

Find the process:

```bash
ps aux | grep azurite
```

Stop by PID:

```bash
kill <pid>
```

Or stop by command pattern:

```bash
pkill -f "azurite --location .azurite"
```

Verify it stopped:

```bash
lsof -n -P -iTCP -sTCP:LISTEN | grep -E ':(10000|10001|10002)\b'
```

Expected result:
- no matching listeners

---

## 7. Reset Azurite State

Use this when you want a clean local storage state.

### Step 1: stop Azurite

```bash
pkill -f "azurite --location .azurite"
```

### Step 2: remove repository-local Azurite data

```bash
rm -rf .azurite
```

### Step 3: start again

```bash
mkdir -p .azurite
azurite --location .azurite --debug .azurite/debug.log
```

Reset effects:
- deletes local blob, queue, and table emulator state
- removes prior debug logs
- forces local queues/containers/tables to be recreated on next use

---

## 8. Quick Command Reference

### Install

```bash
npm install -g azurite
```

### Start

```bash
cd /Users/tech/dev/net/vehicle-service-booking
mkdir -p .azurite
azurite --location .azurite --debug .azurite/debug.log
```

### Check running

```bash
lsof -n -P -iTCP -sTCP:LISTEN | grep -E ':(10000|10001|10002)\b'
```

### Stop

```bash
pkill -f "azurite --location .azurite"
```

### Reset

```bash
pkill -f "azurite --location .azurite"
rm -rf /Users/tech/dev/net/vehicle-service-booking/.azurite
```
