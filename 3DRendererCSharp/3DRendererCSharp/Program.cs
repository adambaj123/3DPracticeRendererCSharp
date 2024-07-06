using System;
using System.Numerics;
using Tao.FreeGlut;
using OpenGL;
namespace _3DRendererCSharp
{
    class Program
    {
        private static int width = 1200, height = 720;
        private static ShaderProgram program;
        private static VBO<Vector3> cube;
        private static VBO<Vector2> cubeUV;
        private static VBO<Vector3> cubeNormals;
        private static VBO<int> cubeElements;
        private static System.Diagnostics.Stopwatch watch;
        private static float xangle, yangle;
        private static Texture SaveTexture;
        private static bool lighting = true;
        private static bool autoRotate = true;
        private static bool fullscreen = false;
        private static bool up = false, down = false, left = false, right = false;
        static void Main(string[] args)
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("OpenGL Tutorial");
            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);
            Glut.glutCloseFunc(OnClose);
            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutKeyboardUpFunc(OnKeyboardUp);
            Glut.glutReshapeFunc(onReshape);

            Gl.Enable(EnableCap.DepthTest);


            program = new ShaderProgram(VertexShader, FragmentShader);

            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0,1,0)));

            program["light_direction"].SetValue(new Vector3(0, 0, 1));
            //Load Texture
            SaveTexture = new Texture("SaveTexture.jpg");

            cube = new VBO<Vector3>(new Vector3[] {
                new Vector3(1, 1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1),//Front Face
                new Vector3(1, -1, 1), new Vector3(-1, -1, 1), new Vector3(-1, -1, -1), new Vector3(1, -1, -1),
                new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1),
                new Vector3(1, -1, -1), new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1),
                new Vector3(-1, 1, 1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1),
                new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, -1, -1) });

            cubeElements = new VBO<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }, BufferTarget.ElementArrayBuffer);

            cubeUV = new VBO<Vector2>(new Vector2[]{
                new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1),
            new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1),
            new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1),
            new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1),
            new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1),
            new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)});
            cubeNormals = new VBO<Vector3>(new Vector3[]
            {
                new Vector3(0,1,0), new Vector3(0,1,0), new Vector3(0,1,0), new Vector3(0,1,0),
                new Vector3(0,-1,0), new Vector3(0,-1,0), new Vector3(0,-1,0), new Vector3(0,-1,0),
                new Vector3(0,0,1), new Vector3(0,0,1), new Vector3(0,0,1), new Vector3(0,0,1),
                new Vector3(0,0,-1), new Vector3(0,0,-1), new Vector3(0,0,-1), new Vector3(0,0,-1),
                new Vector3(-1,0,0), new Vector3(-1,0,0), new Vector3(-1,0,0), new Vector3(-1,0,0),
                new Vector3(1,0,0), new Vector3(1,0,0), new Vector3(1,0,0), new Vector3(1,0,0)
            }) ;

            

            watch = System.Diagnostics.Stopwatch.StartNew();
            Glut.glutMainLoop();
        }

        private static void OnDisplay()
        {

        }

        private static void OnClose()
        {

            cube.Dispose();
            cubeUV.Dispose();
            cubeElements.Dispose();
            SaveTexture.Dispose();
            cubeNormals.Dispose();
            program.DisposeChildren = true;
            program.Dispose();
        }
        private static void OnRenderFrame()
        {
            watch.Stop();
            float deltaTime = (float)watch.ElapsedTicks/ System.Diagnostics.Stopwatch.Frequency;
            watch.Restart();

            if (autoRotate)
            {
                xangle += deltaTime;
                yangle += deltaTime / 2;
            }
            else
            {
                if (right)
                {
                    yangle += deltaTime;
                }
                if (left)
                {
                    yangle -= deltaTime;
                }
                if (up)
                {
                    xangle += deltaTime;
                }
                if (down)
                {
                    xangle -= deltaTime;
                }
            }
            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Gl.UseProgram(program);
            Gl.BindTexture(SaveTexture);

            uint verexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            Gl.EnableVertexAttribArray(verexPositionIndex);

            program["model_matrix"].SetValue(Matrix4.CreateRotationY(yangle/2) * Matrix4.CreateRotationX(xangle) * Matrix4.CreateTranslation(new Vector3(0, 0, 0)));

            program["enable_lighting"].SetValue(lighting);

            Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeNormals, program, "vertexNormal");
            Gl.BindBufferToShaderAttribute(cubeUV, program, "vertexUV");
            Gl.BindBuffer(cubeElements);

            Gl.DrawElements(BeginMode.Quads, cubeElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Glut.glutSwapBuffers();
        }

        private static void OnKeyboardDown(byte key, int x, int y)
        {
            if (key == 27)
            {

                Glut.glutLeaveMainLoop();
            }
            if(key == 'w')
            {
                up = true;
            }
            if(key == 'a')
            {
                left = true;
            }
            if(key == 's')
            {
                down = true;
            }
            if(key == 'd')
            {
                right = true;
            }
        }

        private static void OnKeyboardUp(byte key, int x, int y)
        {


            if (key == 'l')
            {
                lighting = !lighting;
            }
            if(key == ' ')
            {
                autoRotate = !autoRotate;
            }
            if(key == 'f')
            {
                fullscreen = !fullscreen;

                if (fullscreen)
                {
                    Glut.glutFullScreen();
                }
                else
                {
                    Glut.glutPositionWindow(0, 0);
                    Glut.glutReshapeWindow(1280, 720);
                }
            }
            if (key == 'w')
            {
                up = false;
            }
            if (key == 'a')
            {
                left = false;
            }
            if (key == 's')
            {
                down = false;
            }
            if (key == 'd')
            {
                right = false;
            }
        }

        public static void onReshape(int width, int height)
        {
            Program.width = width;
            Program.height = height;
        }

        public static string VertexShader = @"
#version 130

in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexUV;

out vec2 UV;
out vec3 normal;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void) {
    UV = vertexUV;
    normal = normalize((model_matrix * vec4(vertexNormal, 0)).xyz);
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";
        public static string FragmentShader = @"
#version 130

uniform sampler2D texture;
uniform vec3 light_direction;
uniform bool enable_lighting;

in vec3 normal;
in vec2 UV;

out vec4 fragment;
void main(void)
{
    float diffuse = max(dot(normal, light_direction),0);
    float ambient = 0.3;
    float lighting = (enable_lighting ? max(diffuse, ambient) : 1.0);
    vec4 sample = texture2D(texture, UV);
    fragment = vec4(sample.xyz * lighting, sample.a);
}";
    }


}
