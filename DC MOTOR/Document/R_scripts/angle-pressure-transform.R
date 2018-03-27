setwd("~/Documents/_NILab_CrazyMotor/FacePush/DC MOTOR/Document/Angle 2 Force/")
dir()

dta_lst <- lapply(dir(), function(x) read.table(x, sep = ',', header = TRUE))

# check FacePush was pulling at which trial points 
for ( i in 1:18 ){
  dim(dta_lst[[i]])[1]
  plot(1:dim(dta_lst[[i]])[1], dta_lst[[i]][, 3], type = "l", xlab = i)
  abline(h = mean(dta_lst[[i]][, 3]), col = 2)
}

# extracht data while pushing, for finding pressure
dta_push <- lapply(dta_lst, function(x) {
  subset(x, x[, 3] >= mean(x[, 3]))
})

len_dta_push <- unlist(lapply(dta_push, function(x) dim(x)[1]))

library(data.table)
dta_push <- rbindlist(dta_push)
dta_push$Angle <- rep(seq(from = 10, to = 180, by = 10), len_dta_push)
dta_push
plot(1:1730, dta_push$LowerRight, type = 'l')
dim(dta_push)

library(reshape2)
dta_push_long <- reshape(dta_push,
                         varying = c("UpperRight", "LowerRight", "UpperMiddle",
                                     "LowerLeft", "UpperLeft"),
                         v.names = "Pressure", direction = "long")
dta_push_long$time <- as.factor(dta_push_long$time) 
levels(dta_push_long$time) <- c("UpperRight", "LowerRight", "UpperMiddle",
                                "LowerLeft", "UpperLeft")
names(dta_push_long) <- c("Angle", "Location", "Pressure", "id")


dta_push_long

library(dplyr)

library(ggplot2)
ggplot(dta_push_long, aes(x = Location, y = Pressure, color = Location)) +
  geom_boxplot()