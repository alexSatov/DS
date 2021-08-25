peq=load('eqX2Gt2X1.txt');
peq1=load('eqX2Lt2X1.txt');

%pinf=load('C:\\inf.txt');
c2=load('cycle2.txt');
c3=load('cycle3.txt');
c4=load('cycle4.txt');
c5=load('cycle5.txt');
c6=load('cycle6.txt');
c7=load('cycle7.txt');
c8=load('cycle8.txt');
c9=load('cycle9.txt');
c10=load('cycle10.txt');
c11=load('cycle11.txt');
c12=load('cycle12.txt');
c13=load('cycle13.txt');
c14=load('cycle14.txt');
c15=load('cycle15.txt');

plot(peq(:,1),peq(:,2),'.','Color',[201/255,242/255,242/255],'MarkerSize',2,'DisplayName','eq')
hold on
plot(peq1(:,1),peq1(:,2),'.','Color',[193/255,222/255,198/255],'MarkerSize',2,'DisplayName','eq')

%plot(pinf(:,1),pinf(:,2),'.','Color',[204/255,204/255,204/255],'MarkerSize',2,'DisplayName','inf')
plot(c2(:,1),c2(:,2),'.','Color',[76/255,102/255,0/255],'MarkerSize',2,'DisplayName','2')
plot(c3(:,1),c3(:,2),'.','Color',[255/255,0/255,0/255],'MarkerSize',2,'DisplayName','3')
plot(c12(:,1),c12(:,2),'.','Color',[168/255,2/255,168/255],'MarkerSize',2,'DisplayName','12')
plot(c13(:,1),c13(:,2),'.','Color',[0/255,253/255,255/255],'MarkerSize',2,'DisplayName','13')
plot(c14(:,1),c14(:,2),'.','Color',[254/255,255/255,0/255],'MarkerSize',2,'DisplayName','14')
plot(c15(:,1),c15(:,2),'.','Color',[120/255,55/255,219/255],'MarkerSize',2,'DisplayName','15')
plot(c4(:,1),c4(:,2),'.','Color',[114/255,153/255,0/255],'MarkerSize',2,'DisplayName','4')
plot(c5(:,1),c5(:,2),'.','Color',[134/255,18/255,252/255],'MarkerSize',2,'DisplayName','5')
plot(c6(:,1),c6(:,2),'.','Color',[250/255,145/255,145/255],'MarkerSize',2,'DisplayName','6')
plot(c7(:,1),c7(:,2),'.','Color',[128/255,116/255,98/255],'MarkerSize',2,'DisplayName','7')
plot(c8(:,1),c8(:,2),'.','Color',[0/255,255/255,0/255],'MarkerSize',2,'DisplayName','8')
plot(c9(:,1),c9(:,2),'.','Color',[127/255,0/255,128/255],'MarkerSize',2,'DisplayName','9')
plot(c10(:,1),c10(:,2),'.','Color',[126/255,127/255,247/255],'MarkerSize',2,'DisplayName','10')
plot(c11(:,1),c11(:,2),'.','Color',[0/255,1/255,255/255],'MarkerSize',2,'DisplayName','11')

legend('show','Location','southeastoutside')