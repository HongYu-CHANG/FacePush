force <- c( 0.0375, 0.30875,  0.5800, 0.69375, 0.8075,
            1.1225,  1.4375, 1.70125,  1.9650, 2.2575, 2.5500,
           2.77375,  2.9975,  3.1475, 3.24875, 3.3500)
angle <- seq(from = 30, to = 180, by = 10)

a2f <- data.frame(angle = angle, force = force)

plot(angle, force, type = "b", xlim = c(0, 180), ylim = c(0, 4))

#interpolation <- function(big, small, goal) {
#  b <- which(a2f$angle == big)
#  s <- which(a2f$angle == small)
#  a2f$force[b] - ((big - goal) / (big - small) * (a2f$force[b] - a2f$force[s]))
#}

force_ci <- temp_degree %>% group_by(Tester.Name) %>%
  summarise(mm = mean(Degree))

force_ci <- data.frame(force_ci)
force_ci_new <- vapply(force_ci$mm, function(x) interpolation_new(x), numeric(1))
#force_ci <- c(interpolation(110, 100, force_ci$mm[1]),
#  interpolation(100, 90, force_ci$mm[2]),
#  interpolation(100, 90, force_ci$mm[3]),
#  interpolation(80, 70, force_ci$mm[4]),
#  interpolation(110, 100, force_ci$mm[5]),
#  interpolation(80, 70, force_ci$mm[6]),
#  interpolation(90, 80, force_ci$mm[7]),
#  interpolation(110, 100, force_ci$mm[8]),
#  interpolation(110, 100, force_ci$mm[9]))

# last five trials' mean and sd
m <- mean(force_ci_new)
std <- sd(force_ci_new)
m
m + 1.96 * std / sqrt(9)
m - 1.96 * std / sqrt(9)

#
interpolation_new <- function(goal) {
  small <- goal - (goal %% 10)
  big <- small + 10
  b <- which(a2f$angle == big)
  s <- which(a2f$angle == small)
  a2f$force[b] - ((big - goal) / (big - small) * (a2f$force[b] - a2f$force[s]))
}

#
dta_all$force <- vapply(dta_all$Degree, function(x) interpolation_new(x), numeric(1))

dta_by_trial_force <- dta_all %>% group_by(Tester.Name, Counter) %>%
  summarise(trial_name_m = mean(force)) %>%
  group_by(Counter) %>%
  summarise(trial_m = mean(trial_name_m), trial_sd = sd(trial_name_m))
dta_by_trial_force <- as.data.frame(dta_by_trial_force)
dta_by_trial_force

#
ggplot(dta_by_trial_force, aes(x = Counter, y = trial_m)) +
  geom_errorbar(aes(ymin = trial_m - trial_sd, ymax = trial_m + trial_sd),
                width = .2, size = .3) +
  geom_line() +
  geom_hline(yintercept = m, linetype = 2) +
  geom_point(size = 2) +
  labs( x = "Trial No: 1 - 24", y = "Force in N") + # one side? to KG?
  theme_bw() +
  scale_y_continuous(breaks = seq(0, 4, 0.5), limits = c(0, 4)) 
