Генератор карт, вдохновленный игрой Slay the Spire.

![mapgen](https://user-images.githubusercontent.com/108933370/197626814-d210cb0a-6c9e-4ba6-9201-7cfd576cb6d8.gif)

Алгоритм генерации состоит из следующих шагов:
1. Создается карта точек с помощью алгоритма Poisson Disk Sampling;
2. Точки соединяются между собой с помощью алгоритма Delaunay triangulation;
3. Из треугольников формируется граф;
4. По графу ищем кратчайший путь от старта до финиша по алгоритму Дийктры;
5. Исключаем n случайных точек найденного пути из графа (по желанию, включаем k случайных точек из списка выключенных)
6. Повторяем п. 4-5 несколько раз;
7. Рисуем новый граф только из найденных путей.
8. Для каждого узла графа генерируется одна из локаций в случайном порядке (кроме старта и финиша). Но в моем случае обязательно должно быть 3 сундука и 3 эпичных      врага, остальные локации - обычные враги;
9. Граф отображается на сцене.
 
Как пользоваться:
1. Скачайте преокт и добавьте его через Unity Hub;
2. В Unity запустите сцену SampleScene - сгенерируется карта с параметрами по умолчанию;
3. Параметры генерации можно править в объекте Core на сцене;
4. Исправьте параметры и нажмите "generate new map".

Параметры генерации:
1. Radius - отвечает за размер карты;
2. MinDistanse - минимальное расстояние между точками;
3. PointsPerAttempt - в алгоритме Пуассона: сколько раз точка попытается найти свободное место для генерации. 30 - оптимальное число;
4. TotalIterToFindPath - сколько раз нужно найти кратчаший путь. Чем больше значение, тем ветвистее карта.
5. MaxPointDeactivationForIter - сколько точек исключать из графа на каждой итерации поиска пути; Чем больше значение, тем меньше пути соединяются между собой;
6. MaxPointActivationForIter - сколько точек из списка исключенных снова включать в граф на каждой итерации поиска пути, Чем больше значение, тем больше соединений между    путями;
 
Спасибо:
https://github.com/yurkth/stsmapgen - идея алгоритма генерации;
https://github.com/nol1fe/delaunator-sharp - алгоритм триангуляции Делоне;
https://store.steampowered.com/app/646570/Slay_the_Spire/ - обожаю её;
