#!/usr/bin/env bash

set -e

if [ -z "$1" ]; then
  echo "Usage:"
  echo ""
  echo "  $ $0 name_of_migration_file"
  exit 1
fi

path="src/DataAccess/Migrations/up/$(date -u "+%Y%m%d%H%M%S")_$1.sql"
touch "$path"

echo "Migration created: $path"

# dotnet grate -c "connectionString" -f src/DataAccess/Migrations --dt postgresql -t
# dotnet sqlhydra npgsql --connection-string $HASHSET_CONNECTION_STRING
