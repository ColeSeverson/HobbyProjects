using System;

// Helper methods to calculate position for a single coordinate of the square
double CalculateX(double x, double y, double z, double A, double B, double C) =>
    y * Math.Sin(A) * Math.Sin(B) * Math.Cos(C) - z * Math.Cos(A) * Math.Sin(B) * Math.Cos(C) +
        y * Math.Cos(A) * Math.Sin(C) + z * Math.Sin(A) * Math.Sin(C) + x * Math.Cos(B) * Math.Cos(C);

double CalculateY(double x, double y, double z, double A, double B, double C) =>
    y * Math.Cos(A) * Math.Cos(C) + z * Math.Sin(A) * Math.Cos(C) -
        y * Math.Sin(A) * Math.Sin(B) * Math.Sin(C) + z * Math.Cos(A) * Math.Sin(B) * Math.Sin(C) -
        x * Math.Cos(B) * Math.Sin(C);

double CalculateZ(double x, double y, double z, double A, double B, double C) =>
    z * Math.Cos(A) * Math.Cos(B) - y * Math.Sin(A) * Math.Cos(B) + x * Math.Sin(B);

// Calculate a point on the surface
void CalculateForSurface(double x, double y, double z, double A, double B, double C, int width, int height, int offset, int distance, char character, ref double[] zBuffer, ref char[] drawBuffer) {
    var _x = CalculateX(x, y, z, A, B, C);
    var _y = CalculateY(x, y, z, A, B, C);
    var _z = CalculateZ(x, y, z, A, B, C) + distance;
    
    var oneOverZ = 1/_z;

    var xp = (int)(width / 2.0 + offset * oneOverZ * x * 2.0);
    var yp = (int)(height / 2.0 + 40 * oneOverZ * y);
    var idx = xp + (yp * width);

    if (idx >= 0 && idx < width * height && oneOverZ > zBuffer[idx]) {
        zBuffer[idx] = oneOverZ;
        drawBuffer[idx] = character;
    } 
}

// Helper method to render the visible sections to the screen
void DrawSquare(char[] drawBuffer, int width) {
    for (var i = 0; i < drawBuffer.Length; i++) {
        if (i % width == 0) {
            Console.Write($"{(char)10}");
        } else {
            Console.Write($"{drawBuffer[i]}");
        }
    }
}

// Const variables
const int width = 80, height = 24;
const int distance = 50;
const int cubeWidth = 5;
const char backgroundCharacter = '.';
const int offset = width / 2 - cubeWidth;

// State variables 
char[] buffer;
double[] zBuffer;

// Rotation variables
double A = 0.0, B = 0.0, C = 0.0;
double incrementSpeed = 0.6;

// Entrypoint
while (true) {
    // Clear the screen
    Console.Write("\x1b[2J");

    // Increase rotation state variables 
    A += 0.05;
    B += 0.05;
    C += 0.01;

    // Update the draw buffer
    buffer = Enumerable.Repeat(backgroundCharacter, width * height).ToArray();
    zBuffer = Enumerable.Repeat(0.0, width * height).ToArray();

    for (double x = -cubeWidth; x < cubeWidth; x += incrementSpeed) {
        for (double y = -cubeWidth; y < cubeWidth; y += incrementSpeed) {
            CalculateForSurface(x, y, -cubeWidth, A, B, C, width, height, offset, distance, '@', ref zBuffer, ref buffer);
            CalculateForSurface(cubeWidth, y, x, A, B, C, width, height, offset, distance, '$', ref zBuffer, ref buffer);
            CalculateForSurface(-cubeWidth, y, -x, A, B, C, width, height, offset, distance, '~', ref zBuffer, ref buffer);
            CalculateForSurface(-x, y, cubeWidth, A, B, C, width, height, offset, distance, '#', ref zBuffer, ref buffer);
            CalculateForSurface(x, -cubeWidth, -y, A, B, C, width, height, offset, distance, ';', ref zBuffer, ref buffer);
            CalculateForSurface(x, cubeWidth, y, A, B, C, width, height, offset, distance, '+', ref zBuffer, ref buffer);
        }
    }

    // Draw the square
    DrawSquare(buffer, width);
    Thread.Sleep(80);
}