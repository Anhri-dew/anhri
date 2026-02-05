using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class GameField
    {
        // Размеры поля
        public const int Width = 15;
        public const int Height = 25;

        // Матрица поля: 0 - пусто, 1 - занято
        private int[,] field;

        // Текущая падающая фигура
        public Tetromino CurrentPiece { get; private set; }

        // Счет игры
        public int Score { get; private set; }

        // Уровень игры (увеличивает скорость)
        public int Level { get; private set; }

        // Конструктор
        public GameField()
        {
            field = new int[Height, Width];
            CurrentPiece = new Tetromino();
            Score = 0;
            Level = 1;
        }

        // Проверка столкновения фигуры с полем или другими фигурами
        public bool CheckCollision(Tetromino piece, int offsetX = 0, int offsetY = 0)
        {
            for (int y = 0; y < piece.Height; y++)
            {
                for (int x = 0; x < piece.Width; x++)
                {
                    // Если клетка фигуры занята
                    if (piece.Shape[y, x] == 1)
                    {
                        int fieldX = piece.X + x + offsetX;
                        int fieldY = piece.Y + y + offsetY;

                        // Проверка выхода за границы
                        if (fieldX < 0 || fieldX >= Width || fieldY >= Height)
                            return true;

                        // Проверка столкновения с дном
                        if (fieldY < 0)
                            continue;

                        // Проверка столкновения с другими фигурами
                        if (field[fieldY, fieldX] == 1)
                            return true;
                    }
                }
            }

            return false;
        }

        // Попытка сдвинуть фигуру влево
        public bool MoveLeft()
        {
            if (!CheckCollision(CurrentPiece, -1, 0))
            {
                CurrentPiece.X--;
                return true;
            }
            return false;
        }

        // Попытка сдвинуть фигуру вправо
        public bool MoveRight()
        {
            if (!CheckCollision(CurrentPiece, 1, 0))
            {
                CurrentPiece.X++;
                return true;
            }
            return false;
        }

        // Попытка сдвинуть фигуру вниз
        public bool MoveDown()
        {
            if (!CheckCollision(CurrentPiece, 0, 1))
            {
                CurrentPiece.Y++;
                return true;
            }
            return false;
        }

        // Попытка вращения фигуры
        public bool Rotate()
        {
            int[,] originalShape = (int[,])CurrentPiece.Shape.Clone();
            int originalX = CurrentPiece.X;
    
    // Пробуем повернуть
             CurrentPiece.Rotate();
    
    // Если есть столкновение - пробуем разные позиции (wall kick)
            if (CheckCollision(CurrentPiece))
            {
        // Для I-фигуры особые правила
            if (CurrentPiece.Type == 0) // I-фигура
            {
            // Пробуем разные смещения для I-фигуры
            int[][] wallKicksI = new int[][]
            {
                new int[] { 0, 0 },   // Исходная позиция
                new int[] { -3, 0 },  // Сдвиг влево на 2
                new int[] { 1, 0 },   // Сдвиг вправо на 1
                new int[] { -3, -1 }, // Сдвиг влево-вверх
                new int[] { 1, 2 }    // Сдвиг вправо-вниз
            };

            
            foreach (var kick in wallKicksI)
            {
                CurrentPiece.X = originalX + kick[0];
                CurrentPiece.Y += kick[1];
                
                if (!CheckCollision(CurrentPiece))
                {
                    return true; // Нашли рабочую позицию
                }
            }
        }
        else // Для остальных фигур
        {
            // Стандартные wall kicks
            int[][] wallKicks = new int[][]
            {
                new int[] { 0, 0 },   // Исходная позиция
                new int[] { -1, 0 },  // Сдвиг влево
                new int[] { 1, 0 },   // Сдвиг вправо
                new int[] { 0, -1 },  // Сдвиг вверх
                new int[] { 0, 1 }    // Сдвиг вниз
            };
            
            foreach (var kick in wallKicks)
            {
                CurrentPiece.X = originalX + kick[0];
                CurrentPiece.Y += kick[1];
                
                if (!CheckCollision(CurrentPiece))
                {
                    return true;
                }
            }
        }
        
        // Если ни одна позиция не подошла - возвращаем как было
        CurrentPiece.SetShape(originalShape);
        CurrentPiece.X = originalX;
        return false;
        }
    
        return true;
        }

        // Фиксация фигуры на поле
        public void LockPiece()
        {
            for (int y = 0; y < CurrentPiece.Height; y++)
            {
                for (int x = 0; x < CurrentPiece.Width; x++)
                {
                    if (CurrentPiece.Shape[y, x] == 1)
                    {
                        int fieldX = CurrentPiece.X + x;
                        int fieldY = CurrentPiece.Y + y;

                        if (fieldY >= 0) // Защита от выхода за верхнюю границу
                        {
                            field[fieldY, fieldX] = 1;
                        }
                    }
                }
            }

            // Проверка заполненных линий
            CheckLines();

            ClearInputBuffer();

            // Создание новой фигуры
            CurrentPiece = new Tetromino();

            // ДОБАВЛЯЕМ: сбрасываем буфер ввода консоли
            ClearInputBuffer();
        }

        private void ClearInputBuffer()
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
        }

        // Проверка и удаление заполненных линий
        private void CheckLines()
        {
            int linesCleared = 0;

            for (int y = Height - 1; y >= 0; y--)
            {
                bool lineFull = true;

                // Проверяем, заполнена ли линия
                for (int x = 0; x < Width; x++)
                {
                    if (field[y, x] == 0)
                    {
                        lineFull = false;
                        break;
                    }
                }

                // Если линия заполнена
                if (lineFull)
                {
                    linesCleared++;

                    // Сдвигаем все линии выше вниз
                    for (int yy = y; yy > 0; yy--)
                    {
                        for (int xx = 0; xx < Width; xx++)
                        {
                            field[yy, xx] = field[yy - 1, xx];
                        }
                    }

                    // Очищаем верхнюю линию
                    for (int xx = 0; xx < Width; xx++)
                    {
                        field[0, xx] = 0;
                    }

                    // Снова проверяем эту же позицию (так как теперь там новая линия)
                    y++;
                }
            }

            // Начисление очков
            if (linesCleared > 0)
            {
                // Формула очков как в классическом Тетрисе
                int[] points = { 0, 40, 100, 300, 1200 };
                Score += points[linesCleared] * (Level + 1);

                // Увеличение уровня каждые 10 линий
                Level = (Score / 1000) + 1;
            }
        }

        // Получение состояния клетки поля
        public int GetCell(int y, int x)
        {
            // Если координаты вне поля
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return 0;

            return field[y, x];
        }

        // Проверка конца игры
        public bool IsGameOver()
        {
            // Если новая фигура сразу сталкивается - игра окончена
            return CheckCollision(CurrentPiece);
        }

        // Сброс игры
        public void Reset()
        {
            field = new int[Height, Width];
            CurrentPiece = new Tetromino();
            Score = 0;
            Level = 1;
        }
    }
}
