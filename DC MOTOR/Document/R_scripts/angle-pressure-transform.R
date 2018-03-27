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

# pressure of each fsr
library(ggplot2)
ggplot(dta_push_long, aes(x = reorder(Location, Pressure, mean),
                          y = Pressure, fill = Location)) +
  geom_boxplot() +
  labs(x = "Location on Face", y = "Pressure in kPa") +
  coord_flip() + theme_bw() +
  scale_fill_manual(values = c( "gray25", "gray40", "gray55", "gray70", "gray85")) +
  theme(legend.position = c(0.85, 0.3))

# remove upper middle data
lm(Pressure ~ Angle, data = subset(dta_push_long, dta_push_long$Location != "UpperMiddle"))

library(dplyr)

agg_data <- subset(dta_push_long, dta_push_long$Location != "UpperMiddle")
agg_data <- subset(agg_data, agg_data$Angle != 180)

agg_data <- agg_data %>%  group_by(Angle) %>%
  summarise(m = mean(Pressure))
final_m <- lm(m ~ Angle, agg_data) # the closest to our previous result
summary(final_m)

ggplot(agg_data, aes(x = Angle, y = m)) +
  geom_point(shape = 17, color = "forestgreen", size = 2) +
  #geom_abline(slope = coef(final_m)[2], intercept = coef(final_m)[1]) +
  stat_smooth(method = "lm", se = F, col = "black", lwd = 0.5) +
  labs(x = "Angle 10 - 180 degrees", y = "Pressure in kPa") +
  scale_x_continuous(limits = c(0, 180), breaks = NULL, expand = c(0, 0))+
  scale_y_continuous(limits = c(0, 4.5), breaks = seq(0, 4.5, 0.5), expand = c(0, 0))
  