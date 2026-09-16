# Reliable Order Flow

A production-oriented learning project focused on building a reliable order processing workflow using modern backend architecture patterns.

The goal of this repository is to explore how distributed systems handle failures, consistency, messaging, and reliable communication between services.

## Project Goals

This project is designed to demonstrate enterprise-level backend concepts:

- Building a reliable order processing flow
- Handling distributed transactions without shared databases
- Understanding event-driven architecture
- Implementing message reliability patterns
- Learning how microservices communicate safely
- Applying clean architecture and domain-driven design concepts

## Planned Architecture

The system will evolve around an order workflow with independent services.

Main services:

- Order Service
- Payment Service
- Notification Service (planned)

Each service owns its own data and communicates through asynchronous messaging.

## Technologies

- .NET 8
- C#
- Entity Framework Core
- RabbitMQ
- MassTransit (planned)
- SQL Server / PostgreSQL
- Docker
- Clean Architecture principles

## Reliability Patterns Covered

### Message Delivery

- Publisher / Consumer communication
- Exchange and Queue topology
- Acknowledgement handling
- Redelivery scenarios
- Dead Letter Queue (DLQ)
- Retry policies

### Data Consistency

- Outbox Pattern
- Inbox Pattern
- Idempotent Consumers
- Handling duplicate messages
- Eventually consistent workflows

### Distributed Workflow

Example flow:

```
Order Created
      |
      v
Order Service
      |
      v
OrderCreated Event
      |
      v
Payment Service
      |
      +---- PaymentSucceeded
      |
      +---- PaymentFailed
```

The workflow will handle scenarios such as:

- Service crashes after database commit but before message acknowledgement
- Duplicate event delivery
- Consumer retry processing
- Payment failure compensation

## Learning Roadmap

### Phase 1 - Messaging Fundamentals

- RabbitMQ basics
- Exchanges and queues
- Routing keys
- Consumers
- Manual acknowledgement
- Failure scenarios

### Phase 2 - Reliable Messaging

- Retry mechanisms
- Dead Letter Exchanges
- Duplicate message handling
- Idempotency strategies

### Phase 3 - Enterprise Patterns

- MassTransit integration
- Transactional Outbox
- Inbox Pattern
- Saga Pattern
- Distributed workflow orchestration

### Phase 4 - Production Improvements

- Observability
- Logging
- Metrics
- Health checks
- Containerization
- CI/CD pipeline

## Why This Project Exists

Many backend systems work correctly in simple scenarios, but production systems must handle failures, retries, network problems, and partial execution.

This project focuses on answering practical questions:

- What happens if a service crashes during processing?
- How do we avoid losing messages?
- How do we handle duplicate events?
- How do multiple services stay consistent without sharing a database?

## Status

🚧 Work in progress

This repository is being built step by step while exploring reliable distributed systems patterns and enterprise backend architecture.
