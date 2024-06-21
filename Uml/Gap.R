library(tidyverse)

x <- c("engine", "other")
y <- c("theoretical", "practical")
counts <- c(4, 2, 0, 14)

data <- expand.grid(x = x, y = y) %>% bind_cols(counts = counts)

ggplot(data,
       aes(x = str_to_title(x), 
           y = str_to_title(y),
           colour = str_to_title(x),
           size = counts)) +
  geom_point() +
  geom_text(aes(label = counts), 
            colour = "black", 
            size = 3) +
  scale_x_discrete(position = "bottom") +
  scale_size_continuous(range = c(0, 40)) +
  scale_color_brewer(palette = "Set2") +
  labs(x = NULL, y = NULL) +
  theme(legend.position = "none",
        axis.ticks = element_blank())