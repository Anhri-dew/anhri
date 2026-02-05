using System;
using System.Text;

namespace Tetris
{
    public class GameRenderer
    {
        private const char EmptyCell = ' ';
        private const char FilledCell = '■';
        private const char CurrentPieceCell = '■';
        
        private StringBuilder buffer;
        private string lastFrame;
        
        public GameRenderer()
        {
            buffer = new StringBuilder(2000);
            lastFrame = string.Empty;

            // Не используем SetBufferSize - это вызывает ошибку на не-Windows системах
            try
            {
                // Пробуем установить размер, но не падаем если не получится
                if (OperatingSystem.IsWindows())
                {
                    Console.SetWindowSize(50, 30);
                }
            }
            catch
            {
                // Игнорируем ошибки изменения размера
            }
        }
        
        public void Render(GameField gameField)
        {
            buffer.Clear();
            
            // Заголовок
            buffer.AppendLine("╔══════════════════════════════════╗");
            buffer.AppendLine("║           ТЕТРИС                 ║");
            buffer.AppendLine("╚══════════════════════════════════╝");
            buffer.AppendLine();
            
            // Счет
            buffer.AppendLine($" Счет: {gameField.Score.ToString().PadLeft(8)}");
            buffer.AppendLine($" Уровень: {gameField.Level.ToString().PadLeft(6)}");
            buffer.AppendLine();
            
            // Поле
            DrawFieldToBuffer(gameField, buffer);
            
            // Управление
            buffer.AppendLine();
            buffer.AppendLine(" Управление:");
            buffer.AppendLine(" ← →  - движение");
            buffer.AppendLine(" ↑    - поворот");
            buffer.AppendLine(" ↓    - ускорить падение");
            buffer.AppendLine(" Пробел - сбросить вниз");
            buffer.AppendLine(" P     - пауза");
            buffer.AppendLine(" R     - рестарт");
            buffer.AppendLine(" ESC   - выход");
            
            string currentFrame = buffer.ToString();
            
            // Выводим только если изменилось
            if (currentFrame != lastFrame)
            {
                Console.SetCursorPosition(0, 0);
                Console.Write(currentFrame);
                lastFrame = currentFrame;
            }
        }
        
        private void DrawFieldToBuffer(GameField gameField, StringBuilder buffer)
        {
            buffer.AppendLine(" ╔═══════════════╗");
            
            for (int y = 0; y < GameField.Height; y++)
            {
                buffer.Append(" ║");
                
                for (int x = 0; x < GameField.Width; x++)
                {
                    char cellChar = GetCellChar(gameField, x, y);
                    buffer.Append(cellChar);
                }
                
                buffer.AppendLine("║");
            }
            
            buffer.AppendLine(" ╚═══════════════╝");
        }
        
        private char GetCellChar(GameField gameField, int x, int y)
        {
            // Проверяем текущую фигуру
            Tetromino piece = gameField.CurrentPiece;
            for (int py = 0; py < piece.Height; py++)
            {
                for (int px = 0; px < piece.Width; px++)
                {
                    if (piece.Shape[py, px] == 1 &&
                        piece.X + px == x &&
                        piece.Y + py == y)
                    {
                        return CurrentPieceCell;
                    }
                }
            }
            
            // Проверяем поле
            return gameField.GetCell(y, x) == 1 ? FilledCell : EmptyCell;
        }
        
        public void ShowPauseScreen()
        {
            Console.Clear();
            Console.WriteLine("ПАУЗА. Нажмите P для продолжения");
        }
        
        public void ShowGameOverScreen(int score)
        {
            Console.Clear();
            Console.WriteLine($"ИГРА ОКОНЧЕНА! Счет: {score}");
            Console.WriteLine("Нажмите R для рестарта или ESC для выхода");
        }
    }
}