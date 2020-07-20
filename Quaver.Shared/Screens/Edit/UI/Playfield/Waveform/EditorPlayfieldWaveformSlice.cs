using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quaver.Shared.Assets;
using Wobble.Graphics;
using Wobble.Graphics.Sprites;
using Wobble;
using Quaver.Shared.Scheduling;
using Wobble.Graphics.Shaders;
using System.Collections.Generic;

namespace Quaver.Shared.Screens.Edit.UI.Playfield.Waveform
{
    public class EditorPlayfieldWaveformSlice : Sprite
    {
        private EditorPlayfield Playfield { get; }

        private RenderTarget2D Slice { get; set; }

        private Sprite SliceSprite { get; set; }

        private float[,] SliceData { get; }

        private int SliceSize { get; }

        private double SliceTimeMilliSeconds { get; }

        private double SliceTimeOffset { get; }

        //private Shader SliceShader { get;  }

        public EditorPlayfieldWaveformSlice(EditorPlayfield playfield, int sliceSize, float[,] sliceData, double sliceTime)
        {
            SliceSprite = new Sprite();

            SliceTimeOffset = 0;

            Playfield = playfield;
            SliceSize = sliceSize;
            SliceData = sliceData;
            SliceTimeMilliSeconds = sliceTime + SliceTimeOffset;

            //var (pixelWidth, pixelHeight) = new Vector2((int)playfield.Width, (int)SliceSize) * Wobble.Window.WindowManager.ScreenScale;

            //Slice = new RenderTarget2D(GameBase.Game.GraphicsDevice, (int)pixelWidth, (int)pixelHeight, false,
            //                           GameBase.Game.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);

            //multi threaded do not remove
            ThreadScheduler.Run(() =>
            {
                var sliceTexture = new Texture2D(GameBase.Game.GraphicsDevice, (int)playfield.Width, (int)SliceSize);

                var dataColors = new Color[(int)playfield.Width * (int)SliceSize];

                //double for-loop ouch
                for (var y = 0; y < SliceSize; y += 2)
                {
                    var lengthRight = (int)Math.Abs(SliceData[y, 0] * 127);
                    var lengthLeft = (int)Math.Abs(SliceData[y, 1] * 127);

                    var pivotPoint = (int)playfield.Width / 2 - lengthLeft;

                    for (var x = 0; x < playfield.Width; x++)
                    {
                        var index  = (SliceSize - y - 1) * (int)playfield.Width + x;
                        var index2 = (SliceSize - y - 2) * (int)playfield.Width + x;

                        switch (x >= pivotPoint && x <= pivotPoint + lengthRight + lengthLeft)
                        {
                            case true:
                            dataColors[index].R = 0;
                            dataColors[index].G = 200;
                            dataColors[index].B = 255;
                            dataColors[index].A = 128;

                            dataColors[index2].R = 0;
                            dataColors[index2].G = 200;
                            dataColors[index2].B = 255;
                            dataColors[index2].A = 128;
                                break;

                            default:
                            dataColors[index].R = 0;
                            dataColors[index].G = 0;
                            dataColors[index].B = 0;
                            dataColors[index].A = 0;

                            dataColors[index2].R = 0;
                            dataColors[index2].G = 0;
                            dataColors[index2].B = 0;
                            dataColors[index2].A = 0;
                                break;
                        }
                    }
                }

                sliceTexture.SetData(dataColors);

                ScheduleUpdate(() =>
                {
                    SliceSprite.Image = sliceTexture;
                    SliceSprite.Width = (int)playfield.Width;
                    SliceSprite.Height = SliceSize;
                });
            });

            //single threaded do not remove
            /*GameBase.Game.ScheduledRenderTargetDraws.Add(() =>
            {
                var container = new Container { Size = new ScalableVector2(playfield.Width, SliceSize) };

                var gb = GameBase.Game.GraphicsDevice;

                gb.SetRenderTarget(Slice);
                gb.Clear(Color.TransparentBlack);

                for (var i = 0; i < SliceSize; i += 2)
                {
                    var lengthRight = Math.Abs(SliceData[i, 0] * 128f);
                    var lengthLeft = Math.Abs(SliceData[i, 1] * 128f);

                    var line = new Sprite
                    {
                        Parent = container,
                        Alignment = Alignment.TopLeft,
                        Image = UserInterface.BlankBox,
                        Size = new ScalableVector2(lengthLeft + lengthRight, 2),
                        Position = new ScalableVector2(playfield.Width / 2 - lengthLeft, SliceSize - i),
                        Tint = new Color(0.0f, 0.81f, 1f, 0.75f)
                    };
                }

                container.Draw(new GameTime());

                GameBase.Game.SpriteBatch.End();

                SliceSprite.Image = Slice;
                SliceSprite.Width = (int)playfield.Width;
                SliceSprite.Height = SliceSize;

                SliceSprite.SpriteBatchOptions.Shader = new Shader(GameBase.Game.Resources.Get("Quaver.Resources/Shaders/waveform-slice.mgfxo"), new Dictionary<string, object>() { });

                gb.SetRenderTarget(null);
            });*/
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            SliceSprite.X = Playfield.ScreenRectangle.X;
            SliceSprite.Y = Playfield.HitPositionY - (float)(SliceTimeMilliSeconds + SliceSize) * Playfield.TrackSpeed - Height;

            SliceSprite.Height = SliceSize * Playfield.TrackSpeed;

            SliceSprite.Draw(gameTime);
        }

        /// <summary>
        /// </summary>
        public override void Destroy()
        {
            Slice = null;
            SliceSprite = null;

            base.Destroy();
        }
    }
}