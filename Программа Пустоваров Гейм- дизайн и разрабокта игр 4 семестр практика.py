class Student:
    def __init__(self, name, age, average_score):
        self.name = name
        self.age = age
        self.average_score = average_score

    # Методы для установки значений
    def set_name(self, name):
        self.name = name

    def set_age(self, age):
        self.age = age

    def set_average_score(self, average_score):
        self.average_score = average_score

    # Методы для получения значений
    def get_name(self):
        return self.name

    def get_age(self):
        return self.age

    def get_average_score(self):
        return self.average_score

    # Метод для вывода информации о студенте
    def display_info(self):
        print(f"Имя: {self.name}, Возраст: {self.age}, Средний балл: {self.average_score}")

    # Метод для подсчета оценки из разряда 10-ти бальной шкалы и не меньше 0
    def calculate_grade(self):
        if self.average_score < 10 and self.average_score >= 0:
            if self.average_score >= 8:
                return "Отлично"
            elif 6 <= self.average_score < 8:
                return "Хорошо"
            elif 4 <= self.average_score < 6:
                return "Удовлетворительно"
            else:
                return "Неудовлетворительно"
        else:
            return "Введите коректное значение балла"

    # Дополнительный метод для выфявления может ли студен получать степендию если его оценка хорошо или выше 
    def is_scholarship_eligible(self):
        return self.average_score >= 6

# Создание объектов класса "Студент"
student1 = Student("Иван Иванов", 20, -1)
student2 = Student("Мария Петрова", 21, 9.0)
student3 = Student("Алексей Сидоров", 19, 3.99)
student4 = Student("Алексей Сидоров", 19, 10.1)
student5 = Student("Алексей Сидоров", 19, 6)

# Демонстрация использования
student1.display_info()
print(f"Оценка: {student1.calculate_grade()}")
print(f"Стипендия: {'Да' if student1.is_scholarship_eligible() else 'Нет'}\n")

student2.display_info()
print(f"Оценка: {student2.calculate_grade()}")
print(f"Стипендия: {'Да' if student2.is_scholarship_eligible() else 'Нет'}\n")

student3.display_info()
print(f"Оценка: {student3.calculate_grade()}") 
print(f"Стипендия: {'Да' if student3.is_scholarship_eligible() else 'Нет'}\n")

student4.display_info()
print(f"Оценка: {student4.calculate_grade()}") 
print(f"Стипендия: {'Да' if student4.is_scholarship_eligible() else 'Нет'}\n")

student5.display_info()
print(f"Оценка: {student5.calculate_grade()}") 
print(f"Стипендия: {'Да' if student5.is_scholarship_eligible() else 'Нет'}\n")