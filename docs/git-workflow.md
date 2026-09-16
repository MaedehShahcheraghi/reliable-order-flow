# Git Workflow

## Branch Strategy

The project follows a lightweight Git workflow:

```
main
 |
 |
develop
 |
 +-- feature/*
 +-- fix/*
 +-- refactor/*
```

## Branch Naming

Examples:

```
feature/order-service
feature/payment-consumer
fix/payment-retry
refactor/rabbitmq-abstraction
```

## Commit Convention

Commits should follow Conventional Commits:

```
<type>(scope): description
```

Examples:

```
feat(order): add order creation flow
fix(payment): handle duplicate payment event
refactor(messaging): extract consumer abstraction
test(order): add workflow tests
docs(readme): update architecture documentation
```

## Pull Request Flow

1. Create feature branch
2. Implement changes
3. Create Pull Request
4. CI pipeline must pass
5. Review changes
6. Merge into target branch

## Versioning

The project will follow Semantic Versioning:

```
MAJOR.MINOR.PATCH
```

Example:

```
1.0.0
```
