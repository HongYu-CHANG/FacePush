setwd("~/Documents/_NILab_CrazyMotor/FacePush/DC MOTOR/Applicaton/StudyOne_JND/JND_results/")
file_names <- dir()[grep("csv", dir())]

lst <- lapply(file_names, function(x) read.table(x, header = FALSE, sep = ','))
lst[[1]]

library(data.table)

# do.call("rbind", lst)
dta <- rbindlist(lst)
names(dta) <- c("TrialNo", "Base", "Offset", "Response", "RT")
dta$Subject <- rep(paste0("U", 1:9), each = 32)

library(dplyr)
result <- dta %>% group_by(Base, Offset) %>%
  summarise(Counts = sum(Response))
result <- as.data.frame(result)

result_mat <- round(matrix(result$Counts, c(4, 4), byrow = FALSE) / 18, 2)
dimnames(result_mat) <- list(c(0, 0.125, 0.25, 0.5), c(2.575, 2.7, 2.95, 3.45))


#
dta_1 <- subset(dta, TrialNo < 16)
dta_2 <- subset(dta, TrialNo >= 16)

result_1 <- dta_1 %>% group_by(Base, Offset) %>%
  summarise(Counts = sum(Response))
result_1 <- as.data.frame(result_1)

result_1_mat <- round(matrix(result_1$Counts, c(4, 4), byrow = FALSE) / 9, 2)
dimnames(result_1_mat) <- list(c(0, 0.125, 0.25, 0.5), c(2.575, 2.7, 2.95, 3.45))

result_2 <- dta_2 %>% group_by(Base, Offset) %>%
  summarise(Counts = sum(Response))
result_2 <- as.data.frame(result_2)

result_2_mat <- round(matrix(result_2$Counts, c(4, 4), byrow = FALSE) / 9, 2)
dimnames(result_2_mat) <- list(c(0, 0.125, 0.25, 0.5), c(2.575, 2.7, 2.95, 3.45))

result_mat
abs(1 - result_1_mat)
result_2_mat

heatmap(result_2_mat)
