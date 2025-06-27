/*
 * ==============================================================
 * Raycasting Engine using Raylib.
 * ==============================================================
 */

using System.Numerics;
using Raylib_cs;

namespace FPS;

internal class Program
{
    private const int ScreenWidth = 1280;
    private const int ScreenHeight = 720;
    private const int MapSize = 32;
    private const int AngleSteps = 3600;

    // Lookup tables
    private static readonly double[] SinTable = new double[AngleSteps];
    private static readonly double[] CosTable = new double[AngleSteps];
    private static readonly (double rayDirX, double rayDirY)[] RayTable = new (double, double)[ScreenWidth];

    static void InitTables( double dirX, double dirY, double planeX, double planeY )
    {
        // Sine and Cosine tables
        for( var i = 0; i < AngleSteps; i++ )
        {
            var angle = (i * Math.PI * 2) / AngleSteps;
            SinTable[i] = Math.Sin( angle );
            CosTable[i] = Math.Cos( angle );
        }

        // Ray direction table
        for( var x = 0; x < ScreenWidth; x++ )
        {
            var cameraX = 2.0 * x / ScreenWidth - 1.0;
            RayTable[x] = (dirX + planeX * cameraX, dirY + planeY * cameraX);
        }
    }

    private static void Main( string[] args )
    {
        var map = new int[32, 32]
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 },
            { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            //================================================================================================
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 },
            { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
        };

        // Player Variables
        var playerX = 8.5;
        var playerY = 8.5;
        var playerAngle = 0.0;
        const double moveSpeed = 0.1;
        const double rotSpeed = 0.003;

        // Sine and Cosine tables
        for( var i = 0; i < AngleSteps; i++ )
        {
            var angle = (i * Math.PI * 2) / AngleSteps;
            SinTable[i] = Math.Sin( angle );
            CosTable[i] = Math.Cos( angle );
        }

        // Direction and plane vectors
        var dirX = CosTable[0];
        var dirY = SinTable[0];
        var planeX = -dirY * 0.66;
        var planeY = dirX * 0.66;

        Raylib.InitWindow( ScreenWidth, ScreenHeight, "~THURS~" );
        Raylib.SetTargetFPS( 60 );
        Raylib.DisableCursor();

        var wallTexture = Raylib.LoadTexture( "/home/mikey/RiderProjects/FPS/mossy.png" );


        InitTables( dirX, dirY, planeX, planeY );

        while( !Raylib.WindowShouldClose() )
        {
            // Handle Movement
            var moveX = 0.0;
            var moveY = 0.0;
            if( Raylib.IsKeyDown( KeyboardKey.W ) )
            {
                moveX += dirX * moveSpeed;
                moveY += dirY * moveSpeed;
            }

            if( Raylib.IsKeyDown( KeyboardKey.S ) )
            {
                moveX -= dirX * moveSpeed;
                moveY -= dirY * moveSpeed;
            }

            if( Raylib.IsKeyDown( KeyboardKey.A ) )
            {
                moveX += dirY * moveSpeed;
                moveY -= dirX * moveSpeed;
            }

            if( Raylib.IsKeyDown( KeyboardKey.D ) )
            {
                moveX -= dirY * moveSpeed;
                moveY += dirX * moveSpeed;
            }

            // Collision detection
            const double collisionRadius = 0.2;
            var newX = playerX + moveX;
            var newY = playerY + moveY;

            // Check X
            var mx = (int)(newX + Math.Sign( moveX ) * collisionRadius);
            var my = (int)playerY;
            if( map[my, mx] == 0 ) playerX = newX;

            // Check Y
            mx = (int)playerX;
            my = (int)(newY + Math.Sign( moveY ) * collisionRadius);
            if( map[my, mx] == 0 ) playerY = newY;

            var mouseDelta = Raylib.GetMouseDelta();
            playerAngle += mouseDelta.X * rotSpeed;
            var angleIdx = (int)((playerAngle * AngleSteps / (2 * Math.PI)) % AngleSteps);
            if( angleIdx < 0 ) angleIdx += AngleSteps;
            dirX = CosTable[angleIdx];
            dirY = SinTable[angleIdx];
            planeX = -dirY * 0.66;
            planeY = dirX * 0.66;
            InitTables( dirX, dirY, planeX, planeY );

            Raylib.BeginDrawing();
            Raylib.ClearBackground( Color.Black );

            var ceilingColor = new Color( 15, 15, 15, 255 );
            var floorColor = new Color( 25, 25, 25, 255 );
            const double sigma = 8.0;

            // Raycasting loop
            for( var x = 0; x < ScreenWidth; x++ )
            {
                var rayDirX = RayTable[x].rayDirX;
                var rayDirY = RayTable[x].rayDirY;

                var mapX = (int)playerX;
                var mapY = (int)playerY;
                double sideDistX, sideDistY;
                var deltaDistX = Math.Abs( 1 / rayDirX );
                var deltaDistY = Math.Abs( 1 / rayDirY );
                int stepX, stepY;
                var hit = false;
                var side = 0;

                if( rayDirX < 0 )
                {
                    stepX = -1;
                    sideDistX = (playerX - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0 - playerX) * deltaDistX;
                }

                if( rayDirY < 0 )
                {
                    stepY = -1;
                    sideDistY = (playerY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0 - playerY) * deltaDistY;
                }

                // DDA Loop
                while( !hit )
                {
                    if( sideDistX < sideDistY )
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }

                    if( map[mapY, mapX] == 1 ) hit = true;
                }

                // Fish eye correction
                var t = side == 0
                    ? ((stepX == 1 ? mapX : mapX + 1) - playerX) / rayDirX
                    : ((stepY == 1 ? mapY : mapY + 1) - playerY) / rayDirY;

                var effectiveT = t - collisionRadius;
                if( effectiveT < 0.01 ) effectiveT = 0.01;

                var cosA = dirX * rayDirX + dirY * rayDirY;
                var perpT = effectiveT * cosA;
                if( perpT < 0.01 ) perpT = 0.01;

                var lineHeight = (int)(ScreenHeight / perpT);
                var drawStart = -lineHeight / 2 + ScreenHeight / 2;
                if( drawStart < 0 ) drawStart = 0;
                var drawEnd = lineHeight / 2 + ScreenHeight / 2;
                if( drawEnd >= ScreenHeight ) drawEnd = ScreenHeight - 1;

                Raylib.DrawRectangle( x, 0, 1, drawStart, ceilingColor );


                var wallX = side == 0 ? playerY + t * rayDirY : playerX + t * rayDirX;
                wallX -= Math.Floor( wallX );
                var texWallX = (int)(wallX * wallTexture.Width);
                switch( side )
                {
                    case 0 when rayDirX > 0:
                    case 1 when rayDirY < 0:
                        texWallX = wallTexture.Width - texWallX - 1;
                        break;
                }

                Raylib.DrawRectangle( x, drawEnd, 1, ScreenHeight - drawEnd, floorColor );

                // Dynamic lighting
                var brightness = Math.Exp( -t / sigma );
                var gray = (byte)(brightness * 255);
                var tint = new Color( (int)gray, gray, gray, 255 );

                Rectangle sourceRec = new Rectangle( texWallX, 0, 1, wallTexture.Height );
                Rectangle destRec = new Rectangle( x, drawStart, 1, drawEnd - drawStart );
                Raylib.DrawTexturePro( wallTexture, sourceRec, destRec, new Vector2( 0, 0 ), 0, tint );
            }

            Raylib.EndDrawing();
        }

        Raylib.UnloadTexture( wallTexture );
        Raylib.CloseWindow();
    }
}