# comfortness

setwd("~/Documents/_NILab_CrazyMotor/FacePush/DC MOTOR/Document/Other/")
dir()
dta <- read.table("./comfortness-pilot.csv", sep = ',', header = TRUE)


library(dplyr)
dta$pressure <- as.factor(dta$pressure)
table(dta$pressure)
dta_barplot <- dta %>% group_by(pressure) %>% 
  summarise(m_comfort = mean(comfortness),
            std_comfort = sd(comfortness),
            m_face = mean(from_face),
            std_face = sd(from_face))

# note, change x labels (6 forces), y label?
library(ggplot2)
comfort <- ggplot(dta_barplot, aes(x = pressure, y = m_comfort)) +
  geom_bar(stat = "identity", fill = "gray80") +
  geom_errorbar(aes(ymin = m_comfort - std_comfort,
                    ymax = m_comfort + std_comfort),
                width = .2, color = "gray20") +
  theme_bw() +
  labs(x = "Pressure in kPa", y = "Comfortness 1 - 7") +
  scale_y_continuous(limits = c(0, 7), breaks = seq(1, 7, 1)) +
  theme(axis.text=element_text(size=12), 
        axis.title=element_text(size=14,face="bold"))

face <- ggplot(dta_barplot, aes(x = pressure, y = m_face)) +
  geom_bar(stat = "identity", fill = "gray80") +
  theme_bw() +
  labs(x = "Pressure in kPa", y = "Ratio of the force was came from face") +
  scale_y_continuous(limits = c(0, 1), breaks = seq(0, 1, 0.25)) +
  theme(axis.text=element_text(size=12), 
        axis.title=element_text(size=14,face="bold"))

library(gridExtra)
grid.arrange(comfort, face, nrow = 1)
 