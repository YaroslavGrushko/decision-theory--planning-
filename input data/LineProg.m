% LineProg
% mcc -m LineProg.m
function LineProg;
clear all 
close all
clc %�������� ������� ����������, �������� ���� ����������� ���� � ������� �������

%��������� ������� Aeq �������� �� �����
n_row_Aeq=100; %����� ��������-�����������
n_col_Aeq=40; %����� ���������� ������� �
sourseAeq = fopen('Aeq.txt','r');
B = fscanf(sourseAeq,'%f',[n_col_Aeq n_row_Aeq]);
Aeq = B';
fclose(sourseAeq);

% ��������� ������ beq
soursebeq = fopen('beq.txt','r');
n_row_beq=100; %����� ���������-�����������
beq = fscanf(soursebeq,'%f',[n_row_beq]);
fclose(soursebeq);

%��������� ������� A ���������� �� �����
n_row_A=8; %����� ����������-�����������
n_col_A=40; %����� ���������� ������� �
sourseA = fopen('A.txt','r');
AA = fscanf(sourseA,'%f',[n_col_A n_row_A]);
A = AA';
fclose(sourseA);

% ��������� ��������� ������ b
sourseb = fopen('b.txt','r');
n_row_b=8; %����� ����������-�����������
b = fscanf(sourseb,'%f',[n_row_b]);
fclose(sourseb);

% ��������� ������ ������������� ������� ������� �� �����
sourseC = fopen('C.txt','r');
n_row_C=40; %����� ���������� ������� � ������� �������
C = fscanf(sourseC,'%f',[n_row_C]);
fclose(sourseC);

%C = [-3 -1 5 -3]; %������ ������������� ��� � ������� �������
%A = [1 -3 -4 5;2 -4 0 0;0 0 -9 1]; % ������� ����������-�����������
%b = [9 3 7]; %������ ������ ������ ��������-�����������
%Aeq = [4 0 1 2]; %������� ��������-�����������
%beq = [6]; %������ ������ ������ ��������-�����������

intcon = 1:40; %�������� ��������� �������� ������� � ������� �������
lb = zeros(40,1); %����������� ����� � �����, ������ ������� ������ ����� 4
ub = ones(40,1); %����������� ����� � ������, � ���.��. �������� ��.���.����� 4
f = C; 

exitflag=-2;

while exitflag < 1 %���� �� ����� ���������� ������� (exitflag = 1)
    %��������� �������� ��������� ����������������
    [x,fval,exitflag] = intlinprog(f,intcon,A,b,Aeq,beq,lb,ub);
    % ���� � intlinprog ����������� �����-���� ��������, �� �� ��� ����� ������ [] 
    x ;   %����������� ������
    fval; %����������� ���������
    exitflag;% ������� ��������� ���������� �������
    e=[1; 1; 1; 1; 1; 1; 1; 1];% ���������� ������������ ��������� ������� �� �������
    b = plus(b,e);     
end;

 b = plus(b,-e);% ������� ������

%������� ���������� �� ������ � ����
n_col=8; %����� �������� ������� � (����� ������)
firstname = 'X';
lastname= '.txt';
filename = [firstname lastname];% ������������ �����
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
%������� �������� ������������ ��������� ������� � ���� b_result.txt
fid_b_result = fopen('b_result.txt', 'w');
fprintf(fid_b_result, ' %d \r\n', round(b));
fclose(fid_b_result);
end

