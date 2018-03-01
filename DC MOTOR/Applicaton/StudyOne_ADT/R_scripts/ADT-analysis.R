# data stores at ../ADT_Results

setwd("~/Documents/_NILab_CrazyMotor/FacePush/DC MOTOR/Applicaton/StudyOne_ADT/ADT_Results/")
all_files <- dir() [-c(28:30)]
dta_lst <- lapply(all_files, function(x) read.table(x, header = TRUE, sep = ','))

dta_all <- dta_lst[[1]][c("Tester.Name", "Counter", "Degree")]
for (i in 2:length(all_files)) {
  dta_all <- rbind(dta_all, dta_lst[[i]][c("Tester.Name", "Counter", "Degree")])
}
levels(dta_all$Tester.Name) <- paste0("User ", seq(1:9))

dta_all$Block <- as.factor(rep(all_files, each = 24))
levels(dta_all$Block) <- c("haoran_3_1", "haoran_3_2", "haoran_3_3", 
                           "KT_3_1", "KT_3_2", "KT_3_3",
                           "lolly_3_4", "lolly_3_5", "lolly_3_6",
                           "lynn_3_4", "lynn_3_5", "lynn_3_6",
                           "lywang_3_1", "lywang_3_2", "lywang_3_3",
                           "Ricky_3_1", "Ricky_3_2", "Ricky_3_3", 
                           "Timo_3_1", "Timo_3_2", "Timo_3_3", 
                           "TY_3_4", "TY_3_5", "TY_3_6",
                           "yichen_3_4", "yichen_3_5", "yichen_3_6")
# dta_all$Angle <- as.factor(rep(rep(c(3, 4), times = c(72, 72)), times = 6))

# mean of last five trials
temp_degree <- NULL
for (i in 20:24) {
  temp_degree <- rbind(temp_degree, subset(dta_all, Counter == i))
}

mean(temp_degree$Degree)
sd(temp_degree$Degree) # got problem

mean(dta_by_trial$trial_m[20:24])
sd(dta_by_trial$trial_m[20:24])

last_five_degree_m <- mean(temp_degree$Degree)
last_five_degree_sd <- sd(temp_degree$Degree)
n <- length(temp_degree$Degree)
# calculate CI
last_five_degree_m
last_five_degree_m + 1.96 * last_five_degree_sd / sqrt(n)
last_five_degree_m - 1.96 * last_five_degree_sd / sqrt(n)

library(dplyr)
correct_ci <- temp_degree %>% group_by(Tester.Name) %>%
  summarise(mm = mean(Degree)) %>%
  summarise(m = mean(mm), std = sd(mm))
correct_ci <- as.data.frame(correct_ci)

correct_ci$m
correct_ci$m + 1.96 * correct_ci$std / sqrt(9)
correct_ci$m - 1.96 * correct_ci$std / sqrt(9)

library(ggplot2)
library(gridExtra)

ggplot(dta_all, aes(x = Counter, y = Degree, group = Block)) +
  geom_line() + geom_hline(yintercept = last_five_degree_m, linetype = 2) +
  facet_wrap( ~ Tester.Name) + theme_bw() +
  labs(x = "Trial No: 1 - 24", y = "Degree") +
  scale_y_continuous(breaks = seq(0, 175, 25), limits = c(0, 175)) + 
  theme(legend.position = c(0.8, 0.3))

# change Tester.Name into user 1 to user 9

#wrong <- dta_all %>% group_by(Counter) %>%
#  summarise(trial_m = mean(Degree), trial_sd = sd(Degree))
#as.data.frame(wrong)

dta_by_trial <- dta_all %>% group_by(Tester.Name, Counter) %>%
  summarise(trial_name_m = mean(Degree)) %>%
  group_by(Counter) %>%
  summarise(trial_m = mean(trial_name_m), trial_sd = sd(trial_name_m))
dta_by_trial <- as.data.frame(dta_by_trial)
dta_by_trial

ggplot(dta_by_trial, aes(x = Counter, y = trial_m)) +
  geom_errorbar(aes(ymin = trial_m - trial_sd, ymax = trial_m + trial_sd),
                width = .2, size = .3) +
  geom_line() +
  geom_hline(yintercept = last_five_degree_m, linetype = 2) +
  geom_point(size = 2) +
  #scale_shape(guide = guide_legend(title = NULL)) +
  labs( x = "Trial No: 1 - 24", y = "Mean Degree") +
  theme_bw() +
  scale_y_continuous(breaks = seq(0, 175, 25), limits = c(0, 175)) 