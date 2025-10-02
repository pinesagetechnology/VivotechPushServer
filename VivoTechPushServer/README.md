# VivoTech Push Server

An ASP.NET Core Web API service to receive JSON data from Vivotek push services.

## Endpoints

### Data Endpoint
- **URL**: `/api/vivotek/data`
- **Method**: POST
- **Purpose**: Receives actual data from Vivotek devices

### Logs Endpoint
- **URL**: `/api/vivotek/logs`
- **Method**: POST
- **Purpose**: Receives log data from Vivotek devices

### Health Check
- **URL**: `/api/vivotek/health`
- **Method**: GET
- **Purpose**: Health check endpoint

## Vivotek App Configuration

### For Data Endpoint:
1. **Name**: "Data Endpoint" (or any name you prefer)
2. **Type**: HTTP
3. **Server host**: Your server IP address or domain
4. **Port**: 80 (or your configured port)
5. **Server URI**: `/api/vivotek/data`
6. **Username**: (if authentication required)
7. **Password**: (if authentication required)

### For Logs Endpoint:
1. **Name**: "Logs Endpoint" (or any name you prefer)
2. **Type**: HTTP
3. **Server host**: Your server IP address or domain
4. **Port**: 80 (or your configured port)
5. **Server URI**: `/api/vivotek/logs`
6. **Username**: (if authentication required)
7. **Password**: (if authentication required)

## Running the Service

1. Build the project:
   ```bash
   dotnet build
   ```

2. Run the service:
   ```bash
   dotnet run
   ```

3. The service will be available at:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001` (if configured)

## Testing

You can test the endpoints using curl or Postman:

### Test Data Endpoint:
```bash
curl -X POST "http://localhost:5000/api/vivotek/data" \
  -H "Content-Type: application/json" \
  -d '{
    "timestamp": "2024-01-01T12:00:00Z",
    "device_id": "test-device-001",
    "event_type": "motion_detected",
    "data": {"confidence": 0.95, "region": "zone1"},
    "metadata": {"camera_id": "cam001"}
  }'
```

### Test Logs Endpoint:
```bash
curl -X POST "http://localhost:5000/api/vivotek/logs" \
  -H "Content-Type: application/json" \
  -d '{
    "timestamp": "2024-01-01T12:00:00Z",
    "device_id": "test-device-001",
    "log_level": "INFO",
    "message": "System started successfully",
    "source": "system",
    "details": {"version": "1.0.0"}
  }'
```

## Customization

The service includes placeholder methods `ProcessDataAsync` and `ProcessLogAsync` where you can implement your specific business logic for handling the received data and logs.
