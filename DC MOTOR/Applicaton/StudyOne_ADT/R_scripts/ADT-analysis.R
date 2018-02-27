# data stores at ../ADT_Results

setwd("~/Documents/_NILab_CrazyMotor/FacePush/DC MOTOR/Applicaton/StudyOne_ADT/./ADT_Results/")
all_files <- dir() [-length(dir())]

dta_lst <- lapply(all_files, function(x) read.table(x, header = TRUE, sep = ','))

dta_all <- dta_lst[[1]][c("Tester.Name", "Counter", "Degree")]
for (i in 2:length(all_files)) {
  dta_all <- rbind(dta_all, dta_lst[[i]][c("Tester.Name", "Counter", "Degree")])
}
dta_all

dta_all$Block <- as.factor(rep(all_files, each = 24))
levels(dta_all$Block) <- c("KT_3_1", "KT_3_2", "KT_3_3", "KT_4_4", "KT_4_5", "KT_4_6",
                           "lolly_3_4", "lolly_3_5", "lolly_3_6", "lolly_4_1", "lolly_4_2", "lolly_4_3",
                           "lynn_3_4", "lynn_3_5", "lynn_3_6", "lynn_4_1", "lynn_4_2", "lynn_4_3",
                           "miao_3_1", "miao_3_2", "miao_3_3", "miao_4_4", "miao_4_5", "miao_4_7",
                           "Ricky_3_1", "Ricky_3_2", "Ricky_3_3", "Ricky_4_4", "Ricky_4_5", "Ricky_4_6",
                           "TY_3_4", "TY_3_5", "TY_3_6", "TY_4_1", "TY_4_2", "TY_4_3")

dta_all$Angle <- as.factor(rep(rep(c(3, 4), times = c(72, 72)), times = 6))

# mean of last five trials
temp_degree <- NULL
for (i in 20:24) {
  temp_degree <- rbind(temp_degree, subset(dta_all, Counter == i))
}

mean(temp_degree$Degree)
sd(temp_degree$Degree)


last_five_degree_m <- mean(subset(temp_degree, Tester.Name != "miao")$Degree)
last_five_degree_sd <- sd(subset(temp_degree, Tester.Name != "miao")$Degree)
n <- length(subset(temp_degree, Tester.Name != "miao")$Degree)
# calculate CI
last_five_degree_m
last_five_degree_m + 1.96 * last_five_degree_sd / sqrt(n)
last_five_degree_m - 1.96 * last_five_degree_sd / sqrt(n)

library(ggplot2)
library(gridExtra)

ggplot(subset(dta_all, Tester.Name != "miao"), aes(x = Counter, y = Degree, group = Block, color = Angle)) +
  geom_line() + geom_hline(yintercept = last_five_degree_m, linetype = 2) +
  facet_wrap( ~ Tester.Name) + theme_bw() +
  labs(x = "Trial No: 1 - 24", y = "Degree") +
  scale_y_continuous(breaks = seq(0, 175, 25), limits = c(0, 175)) + 
  theme(legend.position = c(0.8, 0.3))

