# HR MobyPark - Docker Deployment

This repository contains a .NET 8.0 application with Docker containerization and automated deployment via GitHub Actions.

## Docker Setup

### Dockerfile
The `Dockerfile` uses a multi-stage build approach:
- **Build Stage**: Uses `mcr.microsoft.com/dotnet/sdk:8.0` to build the application
- **Runtime Stage**: Uses `mcr.microsoft.com/dotnet/aspnet:8.0` for a lightweight production image

### Docker Compose
The `docker-compose.yml` file manages the container deployment:
- Exposes the application on port **8080**
- Sets up a dedicated bridge network
- Configures restart policy as `unless-stopped`

## Local Development

### Building the Docker Image
```bash
docker build -t mobypark:latest .
```

### Running with Docker Compose
```bash
docker-compose up -d
```

The application will be available at `http://localhost:8080`

### Stopping the Container
```bash
docker-compose down
```

## Automated Deployment

### GitHub Actions Workflow
The workflow (`.github/workflows/dotnet.yml`) is triggered on push to the `live` branch and performs:

1. **Build & Test**: Builds and tests the .NET application
2. **Docker Build**: Creates a Docker image tagged with `latest` and the commit SHA
3. **Deploy**: Deploys to a remote self-hosted server via SSH

### Deployment Process
The deployment step:
1. Stops and removes the old running container
2. Transfers the Docker image and docker-compose.yml to the remote server
3. Moves files to the deployment directory (`/opt/mobypark`)
4. Loads the new Docker image
5. Starts the container using Docker Compose
6. Verifies the deployment

### Required Secrets
Configure these secrets in your GitHub repository settings:

- `SSH_PRIVATE_KEY`: Private SSH key for accessing the remote server
- `SSH_HOST`: Hostname or IP address of the remote server
- `SSH_USER`: SSH username for the remote server

### Remote Server Requirements
The remote server must have:
- Docker installed and running
- Docker Compose installed
- SSH access configured
- Port 8080 available
- User must have sudo privileges (or `/opt/mobypark` directory must be writable)
- Deployment directory: `/opt/mobypark`

## Architecture

- **Application Port**: 8080
- **Container Name**: mobypark-app
- **Image Name**: mobypark:latest
- **Network**: mobypark-network (bridge)

## Environment Variables

The application uses the following environment variables:
- `ASPNETCORE_URLS`: http://+:8080
- `ASPNETCORE_ENVIRONMENT`: Production

## Troubleshooting

### View Container Logs
```bash
docker logs mobypark-app
```

### Check Container Status
```bash
docker ps | grep mobypark
```

### Restart Container
```bash
docker-compose restart
```

### Rebuild and Restart
```bash
docker-compose down
docker-compose up -d --build
```
