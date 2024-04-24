# Set the current working directory
setwd("D:\\Git\\10-Master\\Experiments\\Experiment1_Results\\Sequential")

# Read data from files
rest <- as.numeric(readLines("Rest.txt"))
kafka <- as.numeric(readLines("Kafka.txt"))
rabbit <- as.numeric(readLines("Rabbit.txt"))

# Print summary statistics
print(summary(rest))
print(summary(kafka))
print(summary(rabbit))

# Create boxplots
boxplot(rest, kafka, rabbit, names = c("Rest", "Kafka", "Rabbit"), 
        main = "Boxplot of Kafka, Rabbit, and Rest",
        xlab = "Data Sets", ylab = "Values")
