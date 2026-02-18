using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Geometry_Dash.GameLogic
{
    public class Player
    {
        public PictureBox Box { get; private set; }
        public float VelocityY { get; set; } = 0f;
        public bool IsGrounded { get; set; } = false;
        public bool ShouldRotate { get; set; } = false;
        public bool IsTouchingOrb { get; set; } = false;
        public bool IsGravityInverted { get; set; } = false;

        private readonly float gravity = 0.5f;
        private readonly float jumpStrength = -8f;

        private const float JumpDurationTicks = 30f; // Тривалість анімації стрибка в тіках
        private const float FullRotationAngle = 270f; // Максимальний кут повороту під час стрибка
        private const float RotationStepAngle = 30f;  // Крок повороту для вибору кадру

        private readonly Bitmap originalImage;
        private List<Bitmap> rotationFrames = new();
        private float jumpElapsed = 0f;
        private float rotationAngle = 0;

        // Додаємо константу замість "магічного" рядка
        private const string PlayerImagePath = "Resources/player_block.png";

        public Player(Control.ControlCollection controls, Size clientSize)
        {
            Box = new PictureBox
            {
                Width = 40,
                Height = 40,
                Left = 100,
                Top = clientSize.Height - 70,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            try
            {
                // Використовуємо константу
                originalImage = new Bitmap(PlayerResourcePath); 
                Box.Image = originalImage;
            }
            catch
            {
                MessageBox.Show("Не вдалося завантажити player_block.png", "Помилка ресурсу");
                originalImage = new Bitmap(PlayerImagePath);
                Box.BackColor = Color.Red; // 🟥 Тимчасовий колір для тесту

                Box.Image = originalImage; // НЕ встановлюй червоний фон!
            }

            PrecomputeRotations();
            controls.Add(Box);
        }


        public void Jump()
        {
            if (IsGrounded || IsTouchingOrb)
            {
                float power = IsTouchingOrb ? 11f : 8f;

                // 🔁 Стрибок у правильному напрямку в залежності від гравітації
                VelocityY = IsGravityInverted ? power : -power;

                IsGrounded = false;
                ShouldRotate = true;
                jumpElapsed = 0f;
                rotationAngle = 0;
                IsTouchingOrb = false;
            }
        }


        public void ApplyGravity()
        {
            VelocityY += gravity;
            Box.Top += (int)(IsGravityInverted ? -VelocityY : VelocityY);
        }


        public void HandleRotation()
        {
            if (!ShouldRotate) return;

            jumpElapsed++;
            float progressJump = jumpElapsed / JumpDurationTicks;

            if (progressJump >= 1f || IsGrounded)
            {
                ShouldRotate = false;
                Box.Image = originalImage;
            }
            else
            {
                rotationAngle = FullRotationAngle * progressJump; 
                RotateImage(rotationAngle);
            }
        }

        public void ResetOrbContact()
        {
            IsTouchingOrb = false;
        }

        private void RotateImage(float angle)
        {
            // Використовуємо константу замість 30f
            int index = Math.Min((int)(angle / RotationStepAngle), rotationFrames.Count - 1);
            Box.Image = rotationFrames[index];
        }

        private void PrecomputeRotations()
        {
            rotationFrames.Clear();

            for (int angle = 0; angle <= 360; angle += 30)
            {
                Bitmap rotated = new(originalImage.Width, originalImage.Height);
                using Graphics g = Graphics.FromImage(rotated);
                g.TranslateTransform(rotated.Width / 2f, rotated.Height / 2f);
                g.RotateTransform(angle);
                g.TranslateTransform(-rotated.Width / 2f, -rotated.Height / 2f);
                g.DrawImage(originalImage, 0, 0);
                rotationFrames.Add(rotated);
            }
        }
    }
}
