setwd("~/Documents/_NILab_CrazyMotor/FacePush/DC MOTOR/Applicaton/Boxer Game/Results/")

# read data
library(data.table)
data_files <- dir()[-c(53, 54)]
dta_lst <- lapply(data_files, function(x) fread(x, sep = ',', stringsAsFactors = FALSE))

# 
dta_rows <- unlist(lapply(dta_lst, function(x) dim(x)[1]))
which(dta_rows == 979)
which(dta_rows == 1396)
data_files[c(33, 40)]

# 
dta <- rbindlist(dta_lst)
str(dta)

dta$send2motor <- as.factor(as.integer(dta$send2motor))

dta$punch_type <- as.factor(dta$punch_type)
levels(dta$punch_type) <- c("heavy_R", "heavy_L", "light_R", "light_L", "light_hook")

dta$motor_data <- as.factor(dta$motor_data)
levels(dta$motor_data) <- c("no_cmd", "back_neutral", "R_light", "R_heavy", "L_light",
                            "M_light", "R_heavy", "M_heavy")
str(dta)

idx <- which(dta$send2motor == TRUE)

idx_shift_0 <- c(0, idx)
idx_shift_1 <- c(idx, 1)

idx_shift_0 - idx_shift_1

interval <- (idx[seq(3, length(idx) - 1, by = 2)] -  idx[seq(1, length(idx) - 1, by = 2)])
mean(interval[-740]) * 0.5


# check with hist
#par(mfrow = c(3, 3))
#lapply(dta, function(x) {
#  if (class(x) == "numeric") { hist(x, xlab = names(x)) }
#  else if (class(x) == "character") { print("no plot for char") }
#  else { plot(x) }
#})

summary(dta)

# add condition and subject
condition_subject <- unlist(lapply(data_files, function(x) unlist(strsplit(x, '[.]'))[1]))
cs_lst <- unlist(lapply(condition_subject, function(x) strsplit(x, '_')))
cs_lst <- as.data.frame(t(matrix(cs_lst, c(2, 52))))

dta$Condition <- rep(cs_lst$V1, dta_rows)
dta$Subject <- rep(cs_lst$V2, dta_rows)

# generate a sequence of no for plot
seq_no <- NULL
for (i in 1:52) {
  seq_no <- c(seq_no, 1:dta_rows[i])
}
dta$seq_no <- seq_no

# adjust rotation x, y, and z of HMD
dta$new_HMD_rot_x <-vapply(dta$HMD_rot.x, function(x) ifelse(x > 180, x - 360, x), numeric(1))
dta$new_HMD_rot_y <-vapply(dta$HMD_rot.y, function(x) ifelse(x > 180, x - 360, x), numeric(1))
dta$new_HMD_rot_z <-vapply(dta$HMD_rot.z, function(x) ifelse(x > 180, x - 360, x), numeric(1))


library(ggplot2)
ggplot(dta, aes(x = seq_no, y = new_HMD_rot_x)) +
  geom_line(alpha = 0.5) +
  facet_grid(Condition ~ Subject)

ggplot(dta, aes(x = seq_no, y = new_HMD_rot_y)) +
  geom_line(alpha = 0.5) +
  facet_grid(Condition ~ Subject)

ggplot(dta, aes(x = seq_no, y = new_HMD_rot_z)) +
  geom_line(alpha = 0.5) +
  facet_grid(Condition ~ Subject)



#
table(dta_sti$Subject)
dta_sti <- subset(dta, dta$send2motor == 1)
ggplot(dta_sti, aes(x = seq_no, y = new_HMD_rot_x)) +
  geom_line(alpha = 0.5) +
  facet_grid(Condition ~ Subject)



# analyse subjective data
dta_rating <- read.table("study2_rating.csv", header = TRUE, sep = ',')
dta_rating <- dta_rating[, -1]
dta_enjoyment <- dta_rating[, c(1, 3, 5, 7, 9)]
dta_realism <- dta_rating[, c(1, 2, 4, 6, 8)]
#
library(reshape2)
#
dta_enjoyment_l <- reshape(dta_enjoyment,
                         varying = c( "A.enjoyment", "B.enjoyment", "C.enjoyment", "D.enjoyment"),
                         v.names = "Enjoyment", direction = "long")
dta_enjoyment_l$time <- as.factor(dta_enjoyment_l$time) 
levels(dta_enjoyment_l$time) <- c("Without FacePush", "Light Force", "Heavy Force", "Two Forces")
names(dta_enjoyment_l) <- c("Subject", "Condition", "Enjoyment", "id")

#
dta_realism_l <- reshape(dta_realism,
                           varying = c( "A.realism", "B.realism", "C.realism", "D.realism"),
                           v.names = "Realism", direction = "long")
dta_realism_l$time <- as.factor(dta_realism_l$time) 
levels(dta_realism_l$time) <- c("Without FacePush", "Light Force", "Heavy Force", "Two Forces")
names(dta_realism_l) <- c("Subject", "Condition", "Realism", "id")

# output of long format
# remove byran
dta_enjoyment_l <- subset(dta_enjoyment_l, Subject != "BRYAN")
dta_realism_l <- subset(dta_realism_l, Subject != "BRYAN")

# not remove brian 
n <- dim(dta_enjoyment_l)[1]
library(dplyr)
dta_enjoyment_plot <- dta_enjoyment_l %>% group_by(Condition) %>%
  summarise(m = mean(Enjoyment), ci = 1.96 * sd(Enjoyment) / sqrt(n), std = sd(Enjoyment))

png("enjoyment-with-one-subject-rm.png", width = 19, height = 16, units = 'cm', res = 300)
ggplot(dta_enjoyment_plot, aes(x = Condition, y = m, fill = Condition)) +
  geom_bar(stat = "identity") +
  geom_errorbar(aes(ymin = m - ci, ymax = m + ci), width = 0.2) +
  scale_y_continuous(breaks = 1:7, limits = c(0, 7)) +
  theme_bw() + labs(x = "", y = "Enjoyment Rating 1 - 7") +
  scale_fill_manual(values = c("gray90", "gray70", "gray50", "gray30")) +
  theme(axis.text = element_text(size = 12),
        axis.title = element_text(size = 14, face = "bold"),
        panel.grid = element_blank(), legend.position = c(0.12, 0.88))
dev.off()

#

dta_realism_plot <- dta_realism_l %>% group_by(Condition) %>%
  summarise(m = mean(Realism), ci = 1.96 * sd(Realism) / sqrt(n), std = sd(Realism))

png("realism-with-one-subject-rm.png", width = 19, height = 16, units = 'cm', res = 300)

ggplot(dta_realism_plot, aes(x = Condition, y = m, fill = Condition)) +
  geom_bar(stat = "identity") +
  geom_errorbar(aes(ymin = m - ci, ymax = m + ci), width = 0.2) +
  scale_y_continuous(breaks = 1:7, limits = c(0, 7)) +
  theme_bw() + labs(x = "", y = "Realism Rating 1 - 7") +
  scale_fill_manual(values = c("gray90", "gray70", "gray50", "gray30")) +
  theme(axis.text = element_text(size = 12),
        axis.title = element_text(size = 14, face = "bold"),
        panel.grid = element_blank(), legend.position = c(0.12, 0.88))

dev.off()

###########################################################
# onw-way repeat measured anova
# 1. Univariate approach using aov()
m0_enjoy <- aov(Enjoyment ~ Condition + Error(Subject/Condition), data = dta_enjoyment_l)
summary(m0_enjoy)

#
m0_realism <- aov(Realism ~ Condition + Error(Subject/Condition), data = dta_realism_l)
summary(m0_realism)
# aov nooooo! use Anova {car} 
library(car)
#--------------------enjoyment--------------------
dta_enjoyment
dta_enjoyment <- subset(dta_enjoyment, X__.1 != "BRYAN")
# 1. use lm on all the repeated-measures variables together
m0_enjoyment <- lm(as.matrix(dta_enjoyment[, 2:5]) ~ 1, data = dta_enjoyment)

# 2. Construct the repeated measures variable
trials = factor(c("A", "B", "C", "D"), ordered = FALSE)
trials

# Then the ‘idata’ parameter is for the repeated-measures part of the data,
# the ‘idesign’ is where you specify the repeated part of the design
m0_out <- Anova(m0_enjoyment, idata = data.frame(trials),
                idesign = ~ trials, type = "III")
summary(m0_out, multivariate = FALSE)
# 3. contrasts with partitioned error
c1 <- t.test(dta_enjoyment$A.enjoyment, dta_enjoyment$B.enjoyment, alternative = "two.sided", mu = 0,
             paired = TRUE)
c2 <- t.test(dta_enjoyment$A.enjoyment, dta_enjoyment$C.enjoyment, alternative = "two.sided", mu = 0,
             paired = TRUE)
c3 <- t.test(dta_enjoyment$A.enjoyment, dta_enjoyment$D.enjoyment, alternative = "two.sided", mu = 0,
             paired = TRUE)
c4 <- t.test(dta_enjoyment$B.enjoyment, dta_enjoyment$C.enjoyment, alternative = "two.sided", mu = 0,
             paired = TRUE)
c5 <- t.test(dta_enjoyment$B.enjoyment, dta_enjoyment$D.enjoyment, alternative = "two.sided", mu = 0,
             paired = TRUE)
c6 <- t.test(dta_enjoyment$C.enjoyment, dta_enjoyment$D.enjoyment, alternative = "two.sided", mu = 0,
             paired = TRUE)

#--------------------realism--------------------
# remove bryan
dta_realism
dta_realism <- subset(dta_realism, X__.1 != "BRYAN")
# 1. use lm on all the repeated-measures variables together
m0_realism <- lm(as.matrix(dta_realism[, 2:5]) ~ 1, data = dta_realism)

# 2. Construct the repeated measures variable
trials = factor(c("A", "B", "C", "D"), ordered = FALSE)
trials

# Then the ‘idata’ parameter is for the repeated-measures part of the data,
# the ‘idesign’ is where you specify the repeated part of the design
m0_out <- Anova(m0_realism, idata = data.frame(trials),
                idesign = ~ trials, type = "III")
summary(m0_out, multivariate = FALSE)
# 3. contrasts with partitioned error
c1 <- t.test(dta_realism$A.realism, dta_realism$B.realism, alternative = "two.sided", mu = 0,
             paired = TRUE)
c2 <- t.test(dta_realism$A.realism, dta_realism$C.realism, alternative = "two.sided", mu = 0,
             paired = TRUE)
c3 <- t.test(dta_realism$A.realism, dta_realism$D.realism, alternative = "two.sided", mu = 0,
             paired = TRUE)
c4 <- t.test(dta_realism$B.realism, dta_realism$C.realism, alternative = "two.sided", mu = 0,
             paired = TRUE)
c5 <- t.test(dta_realism$B.realism, dta_realism$D.realism, alternative = "two.sided", mu = 0,
             paired = TRUE)
c6 <- t.test(dta_realism$C.realism, dta_realism$D.realism, alternative = "two.sided", mu = 0,
             paired = TRUE)