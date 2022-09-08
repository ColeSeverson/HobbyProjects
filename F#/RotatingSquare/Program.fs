open System;

// Helper functions to calculate coordinates for square
let calculateX (i: float) (j: float) (k: float) (A: float) (B: float) (C: float) =
    j * Math.Sin(A) * Math.Sin(B) * Math.Cos(C) - k * Math.Cos(A) * Math.Sin(B) * Math.Cos(C) +
    j * Math.Cos(A) * Math.Sin(C) + k * Math.Sin(A) * Math.Sin(C) + i * Math.Cos(B) * Math.Cos(C)

let calculateY (i: float) (j: float) (k: float) (A: float) (B: float) (C: float) =
    j * Math.Cos(A) * Math.Cos(C) + k * Math.Sin(A) * Math.Cos(C) -
    j * Math.Sin(A) * Math.Sin(B) * Math.Sin(C) + k * Math.Cos(A) * Math.Sin(B) * Math.Sin(C) -
    i * Math.Cos(B) * Math.Sin(C)

let calculateZ (i: float) (j: float) (k: float) (A: float) (B: float) (C: float) =
    k * Math.Cos(A) * Math.Cos(B) - j * Math.Sin(A) * Math.Cos(B) + i * Math.Sin(B)

// Calculate if the surface is visible in the plane of the buffer, ie its coordinates match up with x/y and the z is greater than others
let calculateForSurface (cubeX: float) (cubeY: float) (cubeZ: float) (A: float) (B: float) (C: float) (width: int) (height: int) (character: char) (zBuffer: float[]) (buffer: char[]) = 
    let x: float = calculateX cubeX cubeY cubeZ A B C
    let y: float = calculateY cubeX cubeY cubeZ A B C
    let z: float = (calculateZ cubeX cubeY cubeZ A B C) + 100.0

    let oneOverZ: float = 1.0 / z 

    let xp: int = (int)((width |> float) / 2.0 * oneOverZ * x * 2.0) 
    let yp: int = (int)((height |> float) / 2.0 + 40.0 * oneOverZ * y)
    let idx: int = xp + (yp * width)

    let mutZBuffer: float[] = Array.copy zBuffer
    let mutBuffer: char[] = Array.copy buffer

    if idx >= 0 && idx < width * height && oneOverZ > zBuffer[idx] then 
        mutZBuffer[idx] <- oneOverZ
        mutBuffer[idx] <- character
    
    (mutZBuffer, mutBuffer)

let drawSquare (buffer: char[]) (width: int) =
    Array.iteri (fun index x -> printf "%c" (if index % width = 0 then (char)10 else x)) buffer

[<EntryPoint>]
let main args =
    // Various config variables
    let cubeWidth: int = 20
    let distanceFromCam: int = 100
    let backgroundChar: char = '.'
    let incrementSpeed: float = 0.6

    // The size of my terminal
    let width: int = 80
    let height: int = 24

    // Variables to track current rotation
    let A: float = 0.0
    let B: float = 0.0
    let C: float = 0.0

    printf "\x1b[2J"
    while true do
        // Buffers to hold characters, initialized to 0
        let zBuffer: float[] = Array.zeroCreate (width * height)
        let buffer: char[] = Array.init (width * height) (fun _ -> '.')

        let A = A + 0.05
        let B = B + 0.05
        let C = C + 0.01
        
        // Now we need to iterate and call calculate surface for each surface of the cube
        for cubeX = -cubeWidth to cubeWidth do
            for cubeY = -cubeWidth to cubeWidth do 
            let (zBuffer, buffer) = calculateForSurface cubeX cubeY -cubeWidth A B C width height '@' zBuffer buffer
            let (zBuffer, buffer) = calculateForSurface cubeWidth cubeY cubeX A B C width height '$' zBuffer buffer
            let (zBuffer, buffer) = calculateForSurface -cubeWidth cubeY -cubeX A B C width height '~' zBuffer buffer
            let (zBuffer, buffer) = calculateForSurface -cubeX cubeY cubeWidth A B C width height '#' zBuffer buffer
            let (zBuffer, buffer) = calculateForSurface cubeX -cubeWidth -cubeY A B C width height ';' zBuffer buffer
            let (zBuffer, buffer) = calculateForSurface cubeX cubeWidth cubeY A B C width height '+' zBuffer buffer
            (zBuffer, buffer) |> ignore
            
        // Output the clear command, then draw the square
        printf "\x1b[H"
        drawSquare buffer width
    0