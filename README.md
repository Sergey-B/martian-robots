# Martian Robots

# 🚀 Mars Rover Challenge

## 📝 Description
The surface of Mars can be modelled by a **rectangular grid** around which robots are able to move according to instructions provided from Earth. You are to write a program that determines each sequence of robot positions and reports the final position of the robot.

### 📍 Robot Position & Orientation
A robot position consists of:
* **Grid coordinates**: A pair of integers (`x-coordinate` followed by `y-coordinate`).
* **Orientation**: `N`, `S`, `E`, `W` (North, South, East, and West). 
  * *Note*: The direction North corresponds to the direction from grid point `(x, y)` to grid point `(x, y+1)`.

### ⚙️ Instructions
A robot instruction is a string of the letters `L`, `R`, and `F` which represent:
* **`L` (Left)**: The robot turns left 90 degrees and remains on the current grid point.
* **`R` (Right)**: The robot turns right 90 degrees and remains on the current grid point.
* **`F` (Forward)**: The robot moves forward one grid point in the direction of the current orientation and maintains the same orientation.

> ⚠️ **Extensibility**: There is a possibility that additional command types may be required in the future; provision should be made for this.

### 🌌 Boundary Rules & Scent Mechanism
Since the grid is rectangular and bounded, a robot that moves "off" an edge of the grid is **lost forever**. 

* **Robot Scent**: Lost robots leave a "scent" that prohibits future robots from dropping off the world at the same grid point.
* **Scent Location**: The scent is left at the *last grid position* the robot occupied before disappearing over the edge.
* **Scent Rule**: An instruction to move "off" the world from a grid point from which a robot has been previously lost is **simply ignored** by the current robot.

---

## 📥 Input Specs
* **First line**: The upper-right coordinates of the rectangular world (the lower-left coordinates are assumed to be `0,0`).
* **Remaining lines**: A sequence of robot positions and instructions (**two lines per robot**).
  1. *Line 1*: Initial coordinates and orientation (e.g., `1 1 E`), separated by white space.
  2. *Line 2*: Instruction string containing `L`, `R`, and `F`.
* **Execution**: Each robot is processed sequentially (finishes executing before the next robot begins).
* **Constraints**: 
  * Maximum value for any coordinate is **50**.
  * All instruction strings will be **less than 100 characters** in length.

---

## 📤 Output Specs
For each robot position/instruction in the input, the output should indicate the final grid position and orientation of the robot. 
* If a robot falls off the edge of the grid, the word **`LOST`** should be printed after the position and orientation.

---

## 💻 Example

### Sample Input
```text
5 3
1 1 E
RFRFRFRF
3 2 N
FRRFLLFFRRFLL
0 3 W
LLFFFLFLFL
```

### Sample Output
```text
1 1 E
3 3 N LOST
2 3 S
```

## Assumptions

- No authentication
- No authorization
- No rate limiting
- Data retention policy not needed
- Multiple robot movements sent at once
- Pagination and soring not implemented

## Example

### Step 1: Create a world of size 5×3

```text
POST /api/worlds
Content-Type: application/json

{ "width": 5, "height": 3 }
```

Response (201 Created):

```json
{
  "id": 1,
  "width": 5,
  "height": 3
}
```

### Step 2: Send the first robot

```text
POST /api/worlds/1/robots
Content-Type: application/json

{ "x": 1, "y": 1, "orientation": "E", "instructions": "RFRFRFRF" }
```

Response (200 OK):

```json
{
  "x": 1,
  "y": 1,
  "orientation": "E",
  "lost": false
}
```

### Step 3: Send the second robot

```text
POST /api/worlds/1/robots
Content-Type: application/json

{ "x": 3, "y": 2, "orientation": "N", "instructions": "FRRFLLFFRRFLL" }
```

Response (200 OK):

```json
{
  "x": 3,
  "y": 3,
  "orientation": "N",
  "lost": true
}
```

> Now the world has a scent at position (3, 3).

# Clean Architecture Template

A pragmatic Clean Architecture starter for **.NET 10**. Batteries included, opinionated where it matters, and easy to extend.

## What's included in the template?

- **SharedKernel** project with common Domain-Driven Design abstractions.
- **Domain** layer with sample entities and domain events.
- **Application** layer with abstractions for:
  - CQRS (lightweight, MediatR-free command/query handlers)
  - Example use cases (Todos and Users)
  - Cross-cutting concerns (logging, validation) implemented as decorators
- **Infrastructure** layer with:
  - JWT authentication with **refresh tokens** (with token rotation)
  - Permission-based authorization
  - EF Core + PostgreSQL (snake_case naming, migrations)
  - **HybridCache** for fast, unified caching with cache invalidation
  - Serilog structured logging
- **Web.Api** layer with:
  - Minimal API endpoints
  - **Rate limiting** (configurable global + authentication policies)
  - **OpenTelemetry** tracing and metrics (ASP.NET Core, HTTP, Npgsql, runtime)
  - Global exception handling and `ProblemDetails`
  - Swagger / OpenAPI with JWT support
- **Seq** for searching and analyzing structured logs
  - Seq is available at http://localhost:8081 by default
- **Testing** projects
  - Architecture testing (`ArchitectureTests`)
  - Unit testing (`Application.UnitTests`)
  - Integration testing with **Testcontainers** (`IntegrationTests`)

## Getting started

```bash
docker compose up -d        # PostgreSQL + Seq
dotnet run --project src/Web.Api
```

Run the full test suite (the integration tests spin up a throwaway PostgreSQL container, so
Docker must be running):

```bash
dotnet test MartianRobots.slnx
```

To target .NET 8 or .NET 9 instead of .NET 10, see the notes in `Directory.Build.props`.

I'm open to hearing your feedback about the template and what you'd like to see in future iterations.

If you're ready to learn more, check out [**Pragmatic Clean Architecture**](https://www.milanjovanovic.tech/pragmatic-clean-architecture?utm_source=ca-template):

- Domain-Driven Design
- Role-based authorization
- Permission-based authorization
- Distributed caching with Redis
- OpenTelemetry
- Outbox pattern
- API Versioning
- Unit testing
- Functional testing
- Integration testing

Stay awesome!
