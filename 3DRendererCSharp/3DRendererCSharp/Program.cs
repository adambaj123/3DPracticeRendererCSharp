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
        private static VBO<int> cubeElements;
        private static System.Diagnostics.Stopwatch watch;
        private static float angle;
        private static Texture SaveTexture;
        static void Main(string[] args)
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("OpenGL Tutorial");
            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);
            Glut.glutCloseFunc(OnClose);

            Gl.Enable(EnableCap.DepthTest);


            program = new ShaderProgram(VertexShader, FragmentShader);

            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0,1,0)));

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
            program.DisposeChildren = true;
            program.Dispose();
        }
        private static void OnRenderFrame()
        {
            watch.Stop();
            float deltaTime = (float)watch.ElapsedTicks/ System.Diagnostics.Stopwatch.Frequency;
            watch.Restart();

            angle += deltaTime;
            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Gl.UseProgram(program);
            Gl.BindTexture(SaveTexture);

            uint verexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            Gl.EnableVertexAttribArray(verexPositionIndex);

            program["model_matrix"].SetValue(Matrix4.CreateRotationY(angle/2) * Matrix4.CreateRotationX(angle) * Matrix4.CreateTranslation(new Vector3(0, 0, 0)));
            Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeUV, program, "vertexUV");
            Gl.BindBuffer(cubeElements);

            Gl.DrawElements(BeginMode.Quads, cubeElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Glut.glutSwapBuffers();
        }

        public static string VertexShader = @"
#version 130

in vec3 vertexPosition;
in vec2 vertexUV;

out vec2 UV;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void) {
    UV = vertexUV;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";
        public static string FragmentShader = @"
#version 130

uniform sampler2D texture;

in vec2 UV;

out vec4 fragment;
void main(void)
{
    fragment = texture2D(texture, UV);
}";
    }


}
