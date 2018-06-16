% LineProg
% mcc -m LineProg.m
function LineProg;
clear all 
close all
clc %удаление текущих переменных, закрытие всех графических окон и очистка консоли

%считываем матрицу Aeq равенств из файла
n_row_Aeq=100; %число равенств-ограничений
n_col_Aeq=40; %число параметров вектора х
sourseAeq = fopen('Aeq.txt','r');
B = fscanf(sourseAeq,'%f',[n_col_Aeq n_row_Aeq]);
Aeq = B';
fclose(sourseAeq);

% считываем вектор beq
soursebeq = fopen('beq.txt','r');
n_row_beq=100; %число уравнений-ограничений
beq = fscanf(soursebeq,'%f',[n_row_beq]);
fclose(soursebeq);

%считываем матрицу A неравенств из файла
n_row_A=8; %число неравенств-ограничений
n_col_A=40; %число параметров вектора х
sourseA = fopen('A.txt','r');
AA = fscanf(sourseA,'%f',[n_col_A n_row_A]);
A = AA';
fclose(sourseA);

% считываем начальный вектор b
sourseb = fopen('b.txt','r');
n_row_b=8; %число неравенств-ограничений
b = fscanf(sourseb,'%f',[n_row_b]);
fclose(sourseb);

% считываем вектор коэффициентов целевой функции из файла
sourseC = fopen('C.txt','r');
n_row_C=40; %число параметров вектора х целевой функции
C = fscanf(sourseC,'%f',[n_row_C]);
fclose(sourseC);

%C = [-3 -1 5 -3]; %вектор коэффициентов при х целевой функции
%A = [1 -3 -4 5;2 -4 0 0;0 0 -9 1]; % матрица неравенств-ограничений
%b = [9 3 7]; %вектор правых частей равенств-ограничений
%Aeq = [4 0 1 2]; %матрица равенств-ограничений
%beq = [6]; %вектор правых частей равенств-ограничений

intcon = 1:40; %диапазон изменения индексов вектора х целевой функции
lb = zeros(40,1); %ограничение плана х снизу, задаем нулевой вектор длины 4
ub = ones(40,1); %ограничение плана х сверху, в дан.сл. задается ед.век.длины 4
f = C; 

exitflag=-2;

while exitflag < 1 %пока не будет достигнуто решение (exitflag = 1)
    %запускаем алгоритм линейного программирования
    [x,fval,exitflag] = intlinprog(f,intcon,A,b,Aeq,beq,lb,ub);
    % если в intlinprog отсутствует какой-либо параметр, то на его место ставим [] 
    x ;   %оптимальный вектор
    fval; %оптимальный результат
    exitflag;% признак успешного нахождения решения
    e=[1; 1; 1; 1; 1; 1; 1; 1];% приращение максимальной недельной затраты на единицу
    b = plus(b,e);     
end;

 b = plus(b,-e);% убираем лишнее

%выводим результаты на печать в файл
n_col=8; %число столбцов матрицы Х (число недель)
firstname = 'X';
lastname= '.txt';
filename = [firstname lastname];% конкатенация строк
i=1;
if i==1
    fid = fopen(filename, 'w');
end
if i>1
    fid = fopen(filename, 'at+');
end
switch n_col 
    case  7, fprintf(fid, '%d %d %d %d %d %d %d \r\n', round(x));%matrix X: 7x5
    case  8, fprintf(fid, '%d %d %d %d %d %d %d %d \r\n', round(x));%matrix X: 8x5      
end 
fclose(fid);
%выводим значение максимальной недельной затраты в файл b_result.txt
fid_b_result = fopen('b_result.txt', 'w');
fprintf(fid_b_result, ' %d \r\n', round(b));
fclose(fid_b_result);
end

