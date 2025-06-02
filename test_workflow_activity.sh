#!/bin/bash

# Script to generate realistic workflow activity for testing observability

echo "Generating workflow activity for observability testing..."

BASE_URL="http://localhost:5000"

# Generate some page views
echo "Generating page views..."
for i in {1..5}; do
    curl -s "$BASE_URL/" > /dev/null
    curl -s "$BASE_URL/observability" > /dev/null
    sleep 0.5
done

# Test API endpoints
echo "Testing API endpoints..."
for i in {1..10}; do
    curl -s "$BASE_URL/api/observability/health" > /dev/null
    curl -s "$BASE_URL/api/observability/metrics/summary" > /dev/null
    curl -s "$BASE_URL/metrics" > /dev/null
    sleep 0.2
done

# Generate some workflow operations
echo "Generating workflow operations..."
for i in {1..5}; do
    curl -X POST -H "Content-Type: application/json" \
         -d "{\"action\":\"create\",\"itemId\":\"item-$i\",\"userId\":\"user-$((i % 3 + 1))\"}" \
         "$BASE_URL/api/workflow/dashboard/activity" > /dev/null
    
    curl -X POST -H "Content-Type: application/json" \
         -d "{\"action\":\"approve\",\"itemId\":\"item-$i\",\"userId\":\"manager-1\"}" \
         "$BASE_URL/api/workflow/dashboard/activity" > /dev/null
    
    sleep 1
done

# Test error scenarios
echo "Testing error scenarios..."
curl -s "$BASE_URL/api/nonexistent" > /dev/null
curl -s "$BASE_URL/manager/invalid-page" > /dev/null

# Test tracing
echo "Testing distributed tracing..."
for i in {1..3}; do
    curl -X POST -H "Content-Type: application/json" \
         "$BASE_URL/api/observability/test-trace" > /dev/null
    sleep 0.5
done

echo "Activity generation complete!"
echo "Check your observability dashboards:"
echo "- Piranha Observability: http://localhost:5000/observability"
echo "- Grafana: http://localhost:3000 (admin:admin123)"
echo "- Prometheus: http://localhost:9090"
echo "- Jaeger: http://localhost:16686"
