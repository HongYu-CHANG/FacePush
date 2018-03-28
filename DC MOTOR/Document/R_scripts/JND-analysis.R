setwd("~/Documents/_NILab_CrazyMotor/FacePush/DC MOTOR/Applicaton/StudyOne_JND/JND_results/")
file_names <- dir()[grep("csv", dir())]

lst <- lapply(file_names, function(x) read.table(x, header = FALSE, sep = ','))
#lst[[1]]

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

result_mat <- abs(result_mat - 1)

library(corrplot)
library(RColorBrewer)
# plot the data 
jpeg("out.jpeg", width = 5, height = 5, units = 'in', res = 300)
corrplot(result_mat, method = "color", cl.lim = c(0, 1),
        # col = rev(c(cm.colors(100), cm.colors(100))),
         addgrid.col = "black",# addCoef.col = "black",
         tl.col = "black", tl.srt = 90)
dev.off()

# fitting data with 16 - 4  = 12 points
result_mat
off <- c(0, 0.125, 0.25, 0.5)
base <- c(2.575, 2.7, 2.95, 3.45)

all_off <- outer(off, base, FUN = "+")
# ob_b: offset + base load / base load
ob_b <- sweep(all_off, 2, base, FUN = "/")

out <- data.frame(rate_diff = as.vector(result_mat),
                  delta_load_base = as.vector(ob_b))

# remove 3.45 column for our system limitation
out_remove <- out[1:12, ]
m0_remove <- lm(rate_diff ~ log(delta_load_base), data = out_remove)
summary(m0_remove)

# after fitting the curve, let's try to find 75% and 95% of JND
x <- seq(from = 1, to = 1.3, by = 0.001)
pred_percent_remove <- round(coef(m0_remove)[1] + coef(m0_remove)[2] * log(x), 2)
# delat L / L = 1.21 
x[which(pred_percent_remove == 0.95)]
# delta L / L = 1.135
x[which(pred_percent_remove == 0.75)]

# without remove
m1 <- lm(rate_diff ~ log(delta_load_base), data = out)
summary(m1)
# after fitting the curve, let's try to find 75% and 95% of JND
pred_percent <- round(coef(m1)[1] + coef(m1)[2] * log(x), 2)
# delta L / L = 1.25 
x[which(pred_percent == 0.95)]
#delta L / L = 1.167
x[which(pred_percent == 0.75)]
