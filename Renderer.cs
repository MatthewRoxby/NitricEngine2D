using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using NitricEngine2D.loaders;
using NitricEngine2D.nodes;
using NitricEngine2D.shaders;
using System.Diagnostics;

namespace NitricEngine2D
{
    public static class Renderer
    {
        private static Viewport2D? renderViewport = null;

        public static DefaultSpriteShader defaultSpriteShader = new DefaultSpriteShader();
        public static DefaultViewportShader defaultViewportShader = new DefaultViewportShader();
        public static DefaultParticleShader defaultParticleShader = new DefaultParticleShader();

        public static bool wireframe { get; private set; } = false;

        private static float[] sprite_vertices = new float[]
        {
            -1f,1f,0.0f,
            1f,-1f,0.0f,
            -1f,-1f,0.0f,
            1f,1f,0.0f
        };

        private static float[] sprite_uvs = new float[]
        {
            0.0f,0.0f,
            1.0f,1.0f,
            0.0f,1.0f,
            1.0f,0.0f
        };

        private static uint[] sprite_indices = new uint[]
        {
            2,1,0,1,3,0
        };

        private static int sprite_vao = -1;

        public static void SetWireframe(bool b)
        {
            wireframe = b;
            if (wireframe)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                //GL.Disable(EnableCap.CullFace);
                GL.Disable(EnableCap.Blend);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                //GL.Enable(EnableCap.CullFace);
                GL.Enable(EnableCap.Blend);
            }
        }

        public static void SetViewport(Viewport2D viewport)
        {
            renderViewport = viewport;
            GL.Viewport(0,0, (viewport == null)? GameManager.windowWidth : viewport.width, (viewport == null)? GameManager.windowHeight : viewport.height);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, (viewport == null)? 0 : viewport.frameBuffer);
        }

        public static void RenderSprite(Sprite2D sprite)
        {
            if(renderViewport == null)
            {
                Debug.WriteLine("NO VIEWPORT SET, RENDER SKIPPED");
            }


            if(sprite_vao == -1)
            {
                sprite_vao = MeshLoader.LoadMesh(sprite_vertices, sprite_uvs, sprite_indices);
            }

            Shader shader = sprite.shader;
            if (shader == null) shader = defaultSpriteShader;

            shader.Use();


            //set shader uniforms
            

            Vector2 uv_scale = new Vector2(1f / sprite.h_frames, 1f / sprite.v_frames);

            shader.SetUniform2("uv_scale", uv_scale);

            Vector2 uv_offset = new Vector2(
                uv_scale.X * (sprite.frame % sprite.h_frames),
                uv_scale.Y * (int)(sprite.frame / sprite.h_frames)
            );
            shader.SetUniform2("uv_offset", uv_offset);

            

            shader.SetUniformMatrix("transformation", false, sprite.global_transformation);

            if (renderViewport.renderCamera != null)
            {
                shader.SetUniformMatrix("projection", false, renderViewport.renderCamera.projection);
                shader.SetUniformMatrix("view", false, renderViewport.renderCamera.view);
            }
            else
            {
                Matrix4 m = Matrix4.Identity;
                shader.SetUniformMatrix("projection", false, m);
                shader.SetUniformMatrix("view", false, m);
            }
            

            shader.SetUniform4("modulate", sprite.modulate);

            if(sprite.texture == null)
            {
                shader.SetUniform1("textureEnabled", false);
                shader.SetUniform2("aspect", new Vector2(64,64) * sprite.SPRITE_SCALE);
            }
            else
            {
                shader.SetUniform1("textureEnabled", true);
                shader.SetUniformTexture2D("albedo", 0, sprite.texture);
                shader.SetUniform2("aspect", new Vector2(sprite.texture.width / sprite.h_frames, sprite.texture.height / sprite.v_frames) * sprite.SPRITE_SCALE);
            }
            
            GL.BindVertexArray(sprite_vao);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        public static void RenderViewport(Viewport2D view)
        {
            bool b = wireframe;
            SetWireframe(false);
            if (sprite_vao == -1)
            {
                sprite_vao = MeshLoader.LoadMesh(sprite_vertices, sprite_uvs, sprite_indices);
            }

            Shader shader = (view.shader == null) ? defaultViewportShader : view.shader;

            

            shader.Use();

            //shader.SetUniformMatrix("transformation", false, Matrix4.Identity);
            //shader.SetUniformMatrix("projection", false, Matrix4.Identity);
            //shader.SetUniformMatrix("view", false, Matrix4.Identity);

            Vector2 aspect = Vector2.One;
            if (view.keepAspect)
            {
                float a_x = (float)GameManager.windowWidth / (float)view.width;
                float a_y = (float)GameManager.windowHeight / (float)view.height;


                if(a_x > a_y)
                {
                    aspect.Y = 1.0f;
                    aspect.X = a_y / a_x;
                }
                else
                {
                    aspect.X = 1.0f;
                    aspect.Y = a_x / a_y;
                }
            }

            //Debug.WriteLine(aspect.ToString());

            shader.SetUniform2("aspect", aspect);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, view.colourTexture);

            GL.BindVertexArray(sprite_vao);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            SetWireframe(b);
        }

        public static void RenderParticles(ParticleEmitter emitter)
        {
            if (renderViewport == null)
            {
                Debug.WriteLine("NO VIEWPORT SET, RENDER SKIPPED");
            }


            if (sprite_vao == -1)
            {
                sprite_vao = MeshLoader.LoadMesh(sprite_vertices, sprite_uvs, sprite_indices);
            }

            Shader shader = emitter.shader;
            if (shader == null) shader = defaultParticleShader;

            shader.Use();

            if (emitter.texture == null)
            {
                shader.SetUniform1("textureEnabled", false);
                shader.SetUniform2("aspect", new Vector2(64, 64) * 0.5f);
            }
            else
            {
                shader.SetUniform1("textureEnabled", true);
                shader.SetUniformTexture2D("albedo", 0, emitter.texture);
                shader.SetUniform2("aspect", new Vector2(emitter.texture.width, emitter.texture.height) * 0.5f);
            }

            

            if (renderViewport.renderCamera != null)
            {
                shader.SetUniformMatrix("projection", false, renderViewport.renderCamera.projection);
                shader.SetUniformMatrix("view", false, renderViewport.renderCamera.view);
            }
            else
            {
                Matrix4 m = Matrix4.Identity;
                shader.SetUniformMatrix("projection", false, m);
                shader.SetUniformMatrix("view", false, m);
            }

            GL.BindVertexArray(sprite_vao);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            foreach (Particle p in emitter.GetParticles())
            {
                Matrix4 t = Helper.createTransformation(new Vector2(p.x, p.y), new Vector2(p.s_x, p.s_y), p.rot, emitter.z_index);
                shader.SetUniformMatrix("transformation", false, t);
                shader.SetUniform4("modulate", p.c);

                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            }
        }
    }
}
