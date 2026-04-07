#!/bin/bash

echo "Building BPE5 build..."
dotnet build /p:BPE5=1
cp bin/Debug/netstandard2.1/com.rhythmdr.bpe5editorcoop.dll com.rhythmdr.bpe5editorcoop.dll
echo "Building BPE6 build..."
dotnet build
cp bin/Debug/netstandard2.1/com.rhythmdr.editorcoop.dll com.rhythmdr.editorcoop.dll