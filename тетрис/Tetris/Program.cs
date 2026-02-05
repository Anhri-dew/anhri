using System;
using System.Threading;

namespace Tetris
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Тетрис";
            Console.CursorVisible = false;

            // Создаем объекты игры
            GameField gameField = new GameField();
            GameRenderer renderer = new GameRenderer();

            // Переменные для управления
            bool isRunning = true;
            bool isPaused = false;
            DateTime lastFallTime = DateTime.Now; // Время последнего падения
            int fallInterval = 400; // Интервал падения в миллисекундах (0.5 секунды)

            // Основной игровой цикл
            while (isRunning)
            {
                // Обработка ввода пользователя
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    if (isPaused)
                    {
                        if (key.Key == ConsoleKey.P)
                            isPaused = false;
                    }
                    else
                    {
                        switch (key.Key)
                        {
                            case ConsoleKey.LeftArrow:
                                gameField.MoveLeft();
                                break;

                            case ConsoleKey.RightArrow:
                                gameField.MoveRight();
                                break;

                            case ConsoleKey.DownArrow:
                                // Быстрое падение (игрок нажал вниз)
                                gameField.MoveDown();
                                break;

                            case ConsoleKey.UpArrow:
                                gameField.Rotate();
                                break;

                            case ConsoleKey.Spacebar:
                                // Мгновенное падение до конца
                                while (gameField.MoveDown()) { }
                                gameField.LockPiece();
                                break;

                            case ConsoleKey.P:
                                isPaused = true;
                                break;

                            case ConsoleKey.R:
                                gameField.Reset();
                                break;

                            case ConsoleKey.Escape:
                                isRunning = false;
                                break;
                        }
                    }
                }

                // Если игра на паузе
                if (isPaused)
                {
                    renderer.ShowPauseScreen();
                    Thread.Sleep(100);
                    continue;
                }

                // Проверка конца игры
                if (gameField.IsGameOver())
                {
                    renderer.ShowGameOverScreen(gameField.Score);

                    // Ожидание действия игрока
                    ConsoleKeyInfo key;
                    do
                    {
                        key = Console.ReadKey(true);

                        if (key.Key == ConsoleKey.R)
                        {
                            gameField.Reset();
                            lastFallTime = DateTime.Now; // Сбрасываем таймер
                        }
                        else if (key.Key == ConsoleKey.Escape)
                        {
                            isRunning = false;
                        }
                    } while (key.Key != ConsoleKey.R && key.Key != ConsoleKey.Escape);

                    continue;
                }

                // АВТОМАТИЧЕСКОЕ ПАДЕНИЕ (исправленная логика)
                // Проверяем, прошло ли достаточно времени с последнего падения
                if ((DateTime.Now - lastFallTime).TotalMilliseconds > fallInterval)
                {
                    // Пытаемся сдвинуть фигуру вниз
                    if (!gameField.MoveDown())
                    {
                        // Если не получилось (фигура достигла дна или другой фигуры)
                        gameField.LockPiece(); // Фиксируем фигуру на поле
                    }

                    // Сбрасываем таймер
                    lastFallTime = DateTime.Now;
                }

                // Отрисовка игры
                renderer.Render(gameField);

                // Небольшая задержка для плавности (чтобы не грузить процессор)
                Thread.Sleep(50); // 20 кадров в секунду
            }

            Console.Clear();
            Console.WriteLine("Спасибо за игру!");
            Console.ReadKey();
        }
    }
}
