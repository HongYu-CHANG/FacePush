# comfortness

setwd("~/Documents/_NILab_CrazyMotor/FacePush/DC MOTOR/Document/Other/")
dir()
dta <- read.table("./comfortness-pilot.csv", sep = ',', header = TRUE)

library(dplyr)
dta$pressure <- as.factor(dta$pressure)
table(dta$pressure)
dta %>% group_by(pressure) %>% 
  summarise(m_comfort = mean(comfortness),
            std_comfort = sd(comfortness),
            m_face = mean(from_face),
            std_face = sd(from_face))
