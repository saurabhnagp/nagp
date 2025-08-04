# EmployeeApp - Kubernetes Microservice with PostgreSQL

## Github Url
https://github.com/saurabhnagp/nagp/

## Docker hub image url
https://hub.docker.com/r/saurabhnagp/employee-app-image/tags

### Docker image("employee-app-image") and tag("v5")

### Employee Service web api build in dotnet core 8

### Application Ingress URL
http://34.107.251.8/swagger/index.html



A production-ready, containerized microservice application demonstrating Kubernetes best practices, deployed on Google Cloud Platform (GCP) with PostgreSQL database.

## 📋 Table of Contents

- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Detailed Deployment](#detailed-deployment)
- [API Documentation](#api-documentation)
- [Kubernetes Resources](#kubernetes-resources)
- [Monitoring & Health Checks](#monitoring--health-checks)
- [Fault Tolerance Testing](#fault-tolerance-testing)
- [Troubleshooting](#troubleshooting)
- [Project Structure](#project-structure)

## 🎯 Project Overview

This project demonstrates a complete two-tier microservice architecture:
- **API Service**: .NET 8 Web API with 4 replicas
- **Database**: PostgreSQL StatefulSet with persistent storage
- **Infrastructure**: Kubernetes deployment on GCP with modern DevOps practices

### Key Demonstrations
- ✅ Containerization with Docker
- ✅ Kubernetes deployment with rolling updates
- ✅ External access via Ingress
- ✅ Persistent storage with CSI driver
- ✅ Configuration management with ConfigMaps and Secrets
- ✅ Health checks and fault tolerance
- ✅ Database seeding and connectivity

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Internet      │    │   Ingress       │    │   CLusterIP     │
│                 │    │   Controller    │    │   Service       │
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          └──────────────────────┼──────────────────────┘
                                 │
                    ┌─────────────┴─────────────┐
                    │                           │
            ┌───────▼──────┐           ┌───────▼──────┐
            │   API Pods   │           │  Database    │
            │   (4x)       │           │  Pod (1x)    │
            │              │           │              │
            │ .NET 8 API   │◄──────────┤ PostgreSQL   │
            │              │           │              │
            └──────────────┘           └──────────────┘
                                              │
                                    ┌─────────▼─────────┐
                                    │  Persistent       │
                                    │  Volume (PVC)     │
                                    └───────────────────┘
```

## ✨ Features

### Application Features
- **Employee Management**: CRUD operations for employee records
- **RESTful API**: Standard HTTP endpoints
- **Swagger Documentation**: Interactive API documentation
- **Health Monitoring**: Built-in health and readiness checks

### Kubernetes Features
- **High Availability**: 4 API replicas with rolling updates
- **Zero Downtime**: Rolling update strategy with zero unavailability
- **Persistent Storage**: PostgreSQL data survives pod restarts
- **External Access**: CluserIP service + Ingress routing
- **Configuration Management**: ConfigMaps and Secrets
- **Resource Management**: CPU and memory limits

### DevOps Features
- **Containerization**: Multi-stage Docker build
- **Database Seeding**: Pre-populated with 7 employee records
- **Fault Tolerance**: Automatic pod recovery
- **Monitoring**: Health check endpoints
- **Security**: Database password in Kubernetes Secrets

## 🔧 Prerequisites

### Required Tools
- **Google Cloud SDK** (`gcloud`)
- **Kubectl** (Kubernetes CLI)
- **Docker** (for local testing)
- **Git** (for cloning the repository)

### GCP Requirements
- **Google Cloud Project** with billing enabled
- **GKE (Google Kubernetes Engine)** access
- **Container Registry** access (optional, for custom images)

### Local Development (Optional)
- **.NET 8 SDK** (for local development)
- **PostgreSQL** (for local database testing)

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone <repository-url>
cd EmployeeApp
```

### 2. Set Up GCP Project
```bash
# Set your project ID
export PROJECT_ID="your-gcp-project-id"
gcloud config set project $PROJECT_ID

# Enable required APIs
gcloud services enable container.googleapis.com
gcloud services enable compute.googleapis.com
```

### 3. Create GKE Cluster
```bash
# Create a regional cluster
gcloud container clusters create employee-app-cluster \
  --region=us-central1 \
  --num-nodes=1 \
  --machine-type=e2-medium \
  --enable-autoscaling \
  --min-nodes=1 \
  --max-nodes=3
```

### 4. Get Cluster Credentials
```bash
gcloud container clusters get-credentials employee-app-cluster \
  --region=us-central1
```

### 5. Deploy the Application
```bash
# Deploy database first
kubectl apply -f K8s_Postgres/

# Deploy application
kubectl apply -f k8s/

# Check deployment status
kubectl get pods
kubectl get services
```

### 6. Access the Application
```bash
# Get external IP
kubectl get service employee-app

# Test the API
curl http://EXTERNAL_IP:49971/api/employees
curl http://EXTERNAL_IP:49971/health
```

## 📖 Detailed Deployment

### Step 1: Build and Push Docker Image

#### Option A: Use Pre-built Image
The application uses a pre-built image: `saurabhnagp/employee-app-image:v5`

#### Option B: Build Your Own Image
```bash
# Build the image
docker build -t gcr.io/$PROJECT_ID/employee-app:latest .

# Push to Google Container Registry
docker push gcr.io/$PROJECT_ID/employee-app:latest

# Update deployment to use your image
kubectl set image deployment/employee-app employee-app=gcr.io/$PROJECT_ID/employee-app:latest
```

### Step 2: Deploy Database Layer

```bash
# Apply storage class
kubectl apply -f K8s_Postgres/postgres-storage-class.yaml

# Apply database configuration
kubectl apply -f K8s_Postgres/postgres-configmap.yaml
kubectl apply -f K8s_Postgres/postgres-secret.yaml

# Apply persistent volume claim
kubectl apply -f K8s_Postgres/postgres-volume-claim.yaml

# Apply headless service
kubectl apply -f K8s_Postgres/postgres-headless-service.yaml

# Apply StatefulSet
kubectl apply -f K8s_Postgres/postgres-statefulset.yaml

# (Optional)
# kubectl run -it --rm debug --image=tutum/dnsutils --restart=Never nslookup postgres-headless-service
```

### Step 3: Deploy Application Layer

```bash
# Apply application configuration
kubectl apply -f k8s/employee-app-configmap.yaml
kubectl apply -f k8s/employee-app-secret.yaml

# Apply deployment
kubectl apply -f k8s/employee-app-deployment.yaml

# Apply nginx ingress
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.10.1/deploy/static/provider/cloud/deploy.yaml

# Apply service
kubectl apply -f k8s/employee-app-service.yaml

# Apply ingress
kubectl apply -f k8s/employee-app-ingress.yaml
```

### Step 4: Verify Deployment

```bash
# Check all resources
kubectl get all

# Check pods status
kubectl get pods -o wide

# Check services
kubectl get services

# Check persistent volumes
kubectl get pvc
kubectl get pv

# Check logs
kubectl logs -l app=employee-app
kubectl logs -l app=postgres
```

## 📚 API Documentation

### Base URL
- **Ingress**: `http://34.107.251.8/swagger/index.html` OR `http://employee-app.local` (requires DNS configuration)

### Endpoints

#### 1. Get All Employees
```http
GET /api/employees
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Saurabh"
  },
  {
    "id": 2,
    "name": "Rohit"
  }
  // ... more employees
]
```

#### 2. Add New Employee
```http
POST /api/addemployee
Content-Type: application/json

{
  "name": "John Doe"
}
```

**Response:**
```http
200 OK
```

#### 3. Health Check
```http
GET /health
```

**Response:**
```http
200 OK
Healthy
```

#### 4. Readiness Check
```http
GET /ready
```

**Response:**
```http
200 OK
Ready
```

### Swagger Documentation
Access interactive API documentation at:
```
http://EXTERNAL_IP:49971/swagger
```

## ☸️ Kubernetes Resources

### Application Resources

| Resource | Name | Type | Purpose |
|----------|------|------|---------|
| Deployment | `employee-app` | Deployment | API service with 4 replicas |
| Service | `employee-app` | ClusetIP | service access via ingress |
| Ingress | `employee-app-ingress` | Ingress | Advanced routing |
| ConfigMap | `employee-app-configmap` | ConfigMap | Database connection string |
| Secret | `employee-app-secret` | Secret | Database password |

### Database Resources

| Resource | Name | Type | Purpose |
|----------|------|------|---------|
| StatefulSet | `postgres` | StatefulSet | Database with 1 replica |
| Service | `postgres-headless-service` | Headless Service | Database discovery |
| PVC | `postgres-volume-claim` | PersistentVolumeClaim | Data persistence |
| StorageClass | `postgres-storage-class` | StorageClass | GKE CSI storage |
| ConfigMap | `postgres-configmap` | ConfigMap | Database configuration |
| Secret | `postgres-secret` | Secret | Database password |

### Resource Limits

#### API Service
```yaml
resources:
  requests:
    cpu: 100m
    memory: 128Mi
  limits:
    cpu: 200m
    memory: 256Mi
```

#### Database
```yaml
resources:
  requests:
    cpu: 250m
    memory: 256Mi
  limits:
    cpu: 500m
    memory: 512Mi
```

## 🔍 Monitoring & Health Checks

### Health Check Endpoints
- **Liveness Probe**: `/health` (checks if pod is alive)
- **Readiness Probe**: `/ready` (checks if pod is ready to serve traffic)

### Monitoring Commands
```bash
# Check pod health
kubectl get pods -o wide

# View pod logs
kubectl logs -l app=employee-app
kubectl logs -l app=postgres

# Check resource usage
kubectl top pods

# Monitor deployment status
kubectl rollout status deployment/employee-app
```

## 🧪 Fault Tolerance Testing

### Test Pod Recovery
```bash
# Delete a pod and watch it regenerate
kubectl delete pod -l app=employee-app

# Verify service continues working
curl http://EXTERNAL_IP:49971/api/employees
```

### Test Database Persistence
```bash
# Delete database pod
kubectl delete pod postgres-0

# Wait for pod to restart
kubectl get pods -l app=postgres

# Verify data persists
curl http://EXTERNAL_IP:49971/api/employees
```

## 🔧 Troubleshooting

### Common Issues

#### 1. Pods Not Starting
```bash
# Check pod status
kubectl get pods

# Check pod events
kubectl describe pod <pod-name>

# Check pod logs
kubectl logs <pod-name>
```

#### 2. Database Connection Issues
```bash
# Check database pod
kubectl get pods -l app=postgres

# Check database logs
kubectl logs -l app=postgres

# Test database connectivity
kubectl exec -it postgres-0 -- psql -U postgres -d employeedb
```

#### 3. Service Not Accessible
```bash
# Check service status
kubectl get services

# Check endpoints
kubectl get endpoints
```

#### 4. Storage Issues
```bash
# Check PVC status
kubectl get pvc

# Check PV status
kubectl get pv

# Check storage class
kubectl get storageclass
```

### Useful Commands
```bash
# Get all resources
kubectl get all

# Describe specific resource
kubectl describe deployment employee-app
kubectl describe statefulset postgres

# Check events
kubectl get events --sort-by='.lastTimestamp'

# Check resource usage
kubectl top nodes
kubectl top pods
```

## 📁 Project Structure

```
EmployeeApp/
├── EmployeeApp/                    # .NET 8 Web API
│   ├── Controller/                 # API Controllers
│   │   ├── EmployeeController.cs   # Employee CRUD operations
│   │   └── HealthCheckController.cs # Health check endpoints
│   ├── DataAccess/                 # Data Access Layer
│   │   ├── IDataAccessProvider.cs  # Data access interface
│   │   ├── DataAccessProvider.cs   # Data access implementation
│   │   └── PostgreSqlContext.cs    # Entity Framework context
│   ├── Model/                      # Data Models
│   │   └── Employee.cs             # Employee entity
│   ├── Program.cs                  # Application entry point
│   ├── appsettings.json            # Application configuration
│   └── EmployeeApp.csproj          # Project file
├── k8s/                           # Kubernetes Application Resources
│   ├── employee-app-deployment.yaml # API deployment
│   ├── employee-app-service.yaml   # CluserIP service
│   ├── employee-app-ingress.yaml  # Ingress configuration
│   ├── employee-app-configmap.yaml # Application config
│   └── employee-app-secret.yaml   # Application secrets
├── K8s_Postgres/                  # Kubernetes Database Resources
│   ├── postgres-statefulset.yaml  # Database StatefulSet
│   ├── postgres-headless-service.yaml # Database service
│   ├── postgres-storage-class.yaml # Storage configuration
│   ├── postgres-volume-claim.yaml # Persistent volume claim
│   ├── postgres-configmap.yaml    # Database config
│   └── postgres-secret.yaml       # Database secrets
├── Dockerfile                     # Multi-stage Docker build
├── EmployeeApp.sln                # Solution file
└── README.md                      # This file
```

## 💰 Cost Estimation

### GKE Deployment Costs (Monthly)
- **GKE Cluster**: ~$70
- **Persistent Disk**: ~$5
- **Total**: ~$75/month

### With GCP Free Credits
- **$300 credit**: ~3+ months of usage
- **90 days**: Plenty of time for learning and demonstration


## 📄 License

This project is for educational and demonstration purposes.

## 🆘 Support

For issues and questions:
1. Check the troubleshooting section
2. Review Kubernetes logs
3. Verify GCP project configuration
4. Ensure all prerequisites are met

---

**Happy Deploying! 🚀** 


