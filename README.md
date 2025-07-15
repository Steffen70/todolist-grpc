## TodoList gRPC (Recruiting Edition)

### About This Fork

This repository is a streamlined fork of a student proof-of-concept originally created during a gRPC and ASP.NET learning exercise. The codebase has been cleaned up and kept intentionally small so that it can serve as a **neutral, well-defined backend** for technical interviews at Swisspension.

### Why We Use It

During our three-hour coding assessment each candidate is asked to build a simple front-end that consumes the gRPC API exposed by this project. The task helps us verify practical knowledge claimed on a résumé without relying on any generative AI tools.

-   No ChatGPT, Perplexity, Gemini, or similar assistants are allowed during the session.
-   All development happens on a prepared notebook that already contains the required runtimes and build tools.

### Technology Snapshot

| Layer              | Tech                                                                                             |
| ------------------ | ------------------------------------------------------------------------------------------------ |
| Backend            | C#, .NET, ASP.NET Core, gRPC                                                                     |
| Front-end options  | React, Vue, Angular, Razor Pages, Flask, Laravel, or any framework agreed **before** the session |
| Transport          | HTTP/2 (gRPC) or HTTP/1.1 (gRPC-Web)                                                             |
| Certificate bundle | `cert/localhost.{crt,key,pfx}`                                                                   |

### Your Mission

1. **Clone** the branch prepared for you.
2. **Generate client stubs** for your chosen language or framework by invoking `protoc` with the `protos/todo.proto` file. Detailed flags are intentionally omitted—consult the official docs for your stack.
3. Build a UI that can:
    - Display existing to-do items.
    - Add, update, and delete items through the gRPC service.
4. If you pick a client-side rendered framework, configure a **reverse proxy** so that your UI and the gRPC backend share the same origin. A pre-built _Caddyfile_ is provided to speed this up.

### Certificates & HTTPS

Self-signed certificates for `localhost` live under `cert/`. You may:

-   Import `localhost.pfx` into your browser or operating system trust store, or
-   Reference the `.crt` and `.key` directly from the reverse proxy.

**Note:** The backend is already configured to present this certificate; no extra work is required unless you decide to host the front-end over HTTPS as well.

### Branching Model

Every interviewee works on a dedicated branch named `candidate/<your-name>` that is created before the session starts. Feel free to commit as often as you like—history is part of the evaluation.

### Project Conventions

-   **Commit Messages:** Write each commit message as an English sentence in the past tense (single sentence).
-   **Comments and Variable Naming:** Use English for all code comments and variable names.
-   **Framework and Language Naming Conventions:** Follow the official naming conventions and best practices for the programming language(s) and framework(s) you use in your frontend implementation. This helps maintain readability, consistency, and professionalism in the codebase.

### What We Evaluate

-   Understanding of your chosen front-end stack
-   Ability to read protocol buffers and integrate gRPC or gRPC-Web
-   Code clarity, project structure, and commit hygiene
-   Problem-solving speed without external AI assistance
