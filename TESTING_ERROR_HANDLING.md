# Testing Error Handling & Middleware

## Quick Start Testing

### Test 1: Success Case
```bash
curl -X GET "https://localhost:7123/api/announcements/6500" \
  -H "Accept: application/json"
```
**Expected Response:** 200 OK with announcements data

---

## Error Scenarios

### Test 2: Missing Authority Code (ArgumentException)
```bash
curl -X GET "https://localhost:7123/api/announcements/" \
  -H "Accept: application/json"
```
**Expected Response:** 400 Bad Request
```json
{
  "traceId": "0HN7G1H3FT8D3:00000001",
  "message": "Invalid argument provided.",
  "details": "Authority code cannot be null or empty. (Parameter 'authorityCode')",
  "statusCode": 400,
  "timestamp": "2024-01-15T14:30:46.205Z"
}
```
**Log Output:**
```
[14:30:46.205 +00:00] WRN [BadRequest] [GET] /api/announcements/ | TraceId: 0HN7G1H3FT8D3:00000001
    System.ArgumentException: Authority code cannot be null or empty. (Parameter 'authorityCode')
```

---

### Test 3: Non-Existent Authority (KeyNotFoundException)
```bash
curl -X GET "https://localhost:7123/api/announcements/9999" \
  -H "Accept: application/json"
```
**Expected Response:** 404 Not Found
```json
{
  "traceId": "0HN7G1H3FT8D3:00000002",
  "message": "Resource not found.",
  "details": "Settings for authority '9999' not found.",
  "statusCode": 404,
  "timestamp": "2024-01-15T14:30:47.350Z"
}
```
**Log Output:**
```
[14:30:47.350 +00:00] WRN [NotFound] [GET] /api/announcements/9999 | TraceId: 0HN7G1H3FT8D3:00000002
    System.Collections.Generic.KeyNotFoundException: Settings for authority '9999' not found.
```

---

### Test 4: Valid Authority with Announcements
```bash
curl -X GET "https://localhost:7123/api/announcements/6500" \
  -H "Accept: application/json" \
  -H "Accept-Language: en-US"
```
**Expected Response:** 200 OK with full announcement details
```json
{
  "authorityCode": "6500",
  "authorityName": "עיריית רעננה",
  "announcements": [
    {
      "id": "a-1002",
      "title": "הפסקת מים לשכונה ג'",
      "summary": "יתכנו הפסקות מים זמניות מ-09:00 עד 15:00",
      "categoryName": "מים",
      "isUrgent": true,
      "publishDate": "01/15/2024"
    }
  ]
}
```
**Log Output:**
```
[14:31:00.100 +00:00] INF Request: ? [GET] /api/announcements/6500
[14:31:00.145 +00:00] INF Response: ? [200] /api/announcements/6500 - 45ms
```

---

## Logging Output Examples

### Console Output Format

#### Request Entry
```
14:31:00.100 +00:00 INF Request: ? [GET] /api/announcements/6500
```

#### Success Response
```
14:31:00.145 +00:00 INF Response: ? [200] /api/announcements/6500 - 45ms
```

#### Bad Request Response
```
14:31:05.200 +00:00 INF Request: ? [GET] /api/announcements/
14:31:05.205 +00:00 WRN [BadRequest] [GET] /api/announcements/ | TraceId: 0HN7G1H3FT8D3:00000001
    System.ArgumentException: Authority code cannot be null or empty. (Parameter 'authorityCode')
14:31:05.206 +00:00 WRN Response: ? [400] /api/announcements/ - 6ms
```

#### Server Error Response
```
14:31:10.300 +00:00 INF Request: ? [GET] /api/announcements/6500
14:31:10.315 +00:00 ERR [InternalServerError] [GET] /api/announcements/6500 | TraceId: 0HN7G1H3FT8D3:00000002
    System.NullReferenceException: Object reference not set to an instance of an object.
       at RA_MuniAnnouncements.Application.Services.AnnouncementService.GetNormalizedAnnouncementsAsync(String authorityCode)
14:31:10.316 +00:00 ERR Response: ? [500] /api/announcements/6500 - 16ms
```

---

## Development Mode Testing

When running in Development mode, you'll see:

1. **Full Exception Details**
   - Exception type name
   - Full error message
   - Inner exception information

2. **Pretty-Printed JSON**
   ```json
   {
     "traceId": "...",
     "message": "...",
     "details": "...",
     "statusCode": 400,
     "timestamp": "...",
     "exception": "ArgumentException"
   }
   ```

3. **Debug Level Logs**
   - All Information level logs
   - All Debug level logs
   - Full stack traces

---

## Production Mode Testing

When running in Production mode, you'll see:

1. **Limited Exception Details**
   - User-friendly message only
   - Exception type NOT shown
   - Details may be null

2. **Compact JSON**
   ```json
   {
     "traceId": "...",
     "message": "...",
     "details": null,
     "statusCode": 400,
     "timestamp": "..."
   }
   ```

3. **Information Level Logs**
   - Only Information level logs
   - No Debug level logs
   - Stack traces still logged

---

## Testing with PowerShell

### Test Function
```powershell
function Test-AnnouncementsAPI {
    param(
        [string]$AuthorityCode = "6500",
        [string]$BaseUrl = "https://localhost:7123"
    )

    $url = "$BaseUrl/api/announcements/$AuthorityCode"

    try {
        $response = Invoke-RestMethod -Uri $url -Method Get
        Write-Host "? Success (200)" -ForegroundColor Green
        $response | ConvertTo-Json | Write-Host
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        Write-Host "? Error ($statusCode)" -ForegroundColor Red
        $_.Exception.Response.Content.ReadAsStringAsync().Result | 
            ConvertFrom-Json | ConvertTo-Json | Write-Host
    }
}

# Test success case
Test-AnnouncementsAPI "6500"

# Test not found case
Test-AnnouncementsAPI "9999"

# Test missing parameter (requires URL modification)
```

---

## Testing with cURL - Batch Script

```bash
#!/bin/bash

BASE_URL="https://localhost:7123"
API_ENDPOINT="/api/announcements"

echo "=== Test 1: Success Case ==="
curl -s -X GET "$BASE_URL$API_ENDPOINT/6500" \
  -H "Accept: application/json" | jq .

echo -e "\n=== Test 2: Not Found Case ==="
curl -s -X GET "$BASE_URL$API_ENDPOINT/9999" \
  -H "Accept: application/json" | jq .

echo -e "\n=== Test 3: Empty Authority Code ==="
curl -s -X GET "$BASE_URL$API_ENDPOINT/" \
  -H "Accept: application/json" | jq .
```

---

## Monitoring Logs

### Real-Time Log Monitoring
```bash
# On Windows (if using file logging)
Get-Content -Path "logs.txt" -Wait -Tail 10

# On Linux/Mac
tail -f logs.txt
```

### Filter Logs by Level
```bash
# Get only errors
grep "ERR\|Error" logs.txt

# Get only warnings
grep "WRN\|Warning" logs.txt

# Get only information
grep "INF\|Information" logs.txt
```

### Track by TraceId
```bash
# Get all entries for a specific request
grep "0HN7G1H3FT8D3:00000001" logs.txt
```

---

## Exception Mapping Reference

| Throw This | Get This HTTP Status | Log Level | Example |
|---|---|---|---|
| `new ArgumentException(...)` | 400 | Warning | Invalid format provided |
| `new ArgumentNullException(...)` | 400 | Warning | Required parameter missing |
| `new InvalidOperationException(...)` | 400 | Warning | Operation not valid at this time |
| `new FormatException(...)` | 400 | Warning | Data format incorrect |
| `new KeyNotFoundException(...)` | 404 | Warning | Resource not found |
| `new OperationCanceledException()` | 408 | Warning | Request timeout |
| `new UnauthorizedAccessException(...)` | 401 | Warning | Access denied |
| `new Exception(...)` | 500 | Error | Unexpected error |

---

## Performance Testing

### Response Time Tracking
The middleware automatically logs response times. Example:
```
Response: ? [200] /api/announcements/6500 - 45ms
```

### Load Testing Example
```bash
# Using Apache Bench
ab -n 100 -c 10 "https://localhost:7123/api/announcements/6500"

# Using wrk
wrk -t12 -c400 -d30s https://localhost:7123/api/announcements/6500
```

---

## Integration Testing Scenarios

### Scenario 1: Valid Request Flow
1. User sends GET request with valid authority code
2. Middleware logs request
3. Controller validates input
4. Service processes data
5. Response returned with 200 status
6. Middleware logs response time

### Scenario 2: Validation Error Flow
1. User sends GET request with empty authority code
2. Middleware logs request
3. Controller throws ArgumentException
4. Middleware catches exception
5. Exception logged with context
6. StandardError response returned with 400 status
7. Middleware logs response time

### Scenario 3: Resource Not Found Flow
1. User sends GET request with non-existent authority code
2. Middleware logs request
3. Controller/Service throws KeyNotFoundException
4. Middleware catches exception
5. Exception logged with context
6. Error response returned with 404 status
7. Middleware logs response time

---

## Troubleshooting

### Issue: Middleware not catching exceptions
**Solution:** Ensure `app.UseApplicationMiddleware()` is called early in Program.cs

### Issue: Logs not appearing
**Solution:** Check `builder.AddApplicationLogging()` is called before `app.Build()`

### Issue: Exception details showing in production
**Solution:** Verify `app.Environment.IsProduction()` returns true in production

### Issue: Response time showing as 0ms
**Solution:** This is normal for very fast requests. The stopwatch is accurate to microseconds but logs in milliseconds.
