# Reliable Order Flow

A .NET 10 distributed order processing system designed around reliable messaging, asynchronous communication, and resilient microservice workflows.

This repository focuses on building a production-style backend workflow where services communicate through events and remain reliable in the presence of failures, retries, duplicate messages, and partial execution.

## Overview

The system models an order workflow with independent services:

- Order Service
- Payment Service
- Inventory Service
- Notification Service (planned)

Each service owns its own data and communicates asynchronously through RabbitMQ using MassTransit.

The architecture follows eventual consistency principles instead of relying on distributed database transactions.

---

## Architecture

High-level workflow:

```
Client
  |
  v
Order Service
  |
  | Publish OrderSubmitted
  v
RabbitMQ
  |
  +----------------+
  |                |
  v                v
Payment        Inventory
Service        Service
  |
  |
  | Publish PaymentCompleted
  v
RabbitMQ
  |
  v
Order Service
```

The workflow handles real-world distributed system scenarios such as:

- Service crashes during message processing
- Database changes without losing events
- Duplicate message delivery
- Temporary failures and retries
- Eventual consistency between services

---

## Technology Stack

- .NET 10
- ASP.NET Core 10
- C#
- Entity Framework Core
- PostgreSQL
- RabbitMQ
- MassTransit
- Docker
- GitHub Actions

---

## Reliability Patterns

### Messaging Fundamentals

- RabbitMQ exchanges and queues
- Publish / Subscribe messaging
- Routing and topology
- Message acknowledgement
- Redelivery handling
- Dead Letter Queue

### Reliable Messaging

- Transactional Outbox Pattern
- Inbox Pattern
- Idempotent Consumers
- Duplicate message handling
- Retry strategies
- Error queues

### Distributed Workflow

- Saga Pattern
- Compensation actions
- Long-running business processes
- Eventually consistent workflows

---

## Reliability Problems Addressed

### Database and Message Consistency

The system avoids scenarios where:

```
Save data successfully
        |
        v
Application crashes
        |
        v
Event is never published
```

Transactional Outbox ensures database changes and outgoing events are handled reliably.

---

### Duplicate Message Processing

Distributed systems usually provide at-least-once delivery, which means a message can be delivered more than once.

The system demonstrates how to handle:

```
Message received
      |
      v
Consumer crashes
      |
      v
Message delivered again
```

Inbox and idempotency strategies prevent incorrect repeated processing.

---

## Project Structure

```
src
 |
 +-- Order.Api
 |
 +-- Order.Worker
 |
 +-- Payment.Worker
 |
 +-- Inventory.Worker
 |
 +-- Contracts
```

The structure keeps service boundaries clear while allowing each messaging pattern to be explored independently.

---

## Development Roadmap

### Messaging Layer

- RabbitMQ fundamentals
- Exchange and queue topology
- Consumer acknowledgement
- Retry and redelivery
- Dead Letter handling

### MassTransit Integration

- Consumers
- Publish / Send patterns
- Endpoint configuration
- Error handling
- Middleware pipeline

### Reliability Patterns

- Transactional Outbox
- Consumer Outbox
- Inbox State
- Idempotency
- Duplicate detection

### Distributed Transactions

- Saga orchestration
- Compensation workflows
- Failure recovery strategies

### Production Concerns

- Logging
- Metrics
- Health checks
- Containerization
- CI/CD
- Automated releases

---

## Development Workflow

This repository follows professional engineering practices:

- Conventional Commits
- Pull Request workflow
- GitHub Actions CI
- Semantic Versioning
- Release automation (planned)

---

## Running Locally

Requirements:

- .NET 10 SDK
- Docker
- PostgreSQL
- RabbitMQ

Infrastructure services are provided through Docker Compose.

---

## Purpose

Modern distributed systems are not only about implementing business logic. They must survive failures, network issues, retries, duplicate messages, and partial execution.

This project explores how reliable backend systems are designed and implemented using .NET technologies and messaging patterns.

---

## Status

🚧 Active development
