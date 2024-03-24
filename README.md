# The .NET Implementation of the Saga Pattern

Definition:

This repository provides a sample .NET project that implements the Saga model. The saga pattern is a design model for long-running transactions that span multiple services. It is especially useful in microservice architectures.

The project is a simple e-commerce application that allows users to buy products. The application consists of two services:

It consists of Order service, Stock Service and Pay service.
The saga pattern is applied using the following steps:

The ordering service creates a new order and sends a message to the inventory service.
The inventory service checks the inventory and sends a message to the pay service.
Pay pay service creates a new payment and sends a message back to the ordering service.

If any of the steps fail, the Saga pattern ensures that the transaction is undone.

The project uses the following technologies:

-.NET 8
-ASP.NET Core
-Activity source
-CQRS
-Masstransit
The project is intended to be a learning resource for developers who want to learn the Saga model.

Keywords:

-Saga pattern
-.NET
-Microservices
-Activity source
-CQRS
-Masstransit
