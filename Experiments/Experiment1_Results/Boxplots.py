import os
import numpy as np
import matplotlib.pyplot as plt

# Read data from files
with open("Rest.txt", "r") as file:
    rest = np.array([float(line.strip()) for line in file.readlines()])

with open("Kafka.txt", "r") as file:
    kafka = np.array([float(line.strip()) for line in file.readlines()])

with open("Rabbit.txt", "r") as file:
    rabbit = np.array([float(line.strip()) for line in file.readlines()])

# Print summary statistics
print("Summary statistics for Rest:")
print("Min:", np.min(rest))
print("1st Quartile:", np.percentile(rest, 25))
print("Median:", np.median(rest))
print("Mean:", np.mean(rest))
print("3rd Quartile:", np.percentile(rest, 75))
print("Max:", np.max(rest))

print("\nSummary statistics for Kafka:")
print("Min:", np.min(kafka))
print("1st Quartile:", np.percentile(kafka, 25))
print("Median:", np.median(kafka))
print("Mean:", np.mean(kafka))
print("3rd Quartile:", np.percentile(kafka, 75))
print("Max:", np.max(kafka))

print("\nSummary statistics for Rabbit:")
print("Min:", np.min(rabbit))
print("1st Quartile:", np.percentile(rabbit, 25))
print("Median:", np.median(rabbit))
print("Mean:", np.mean(rabbit))
print("3rd Quartile:", np.percentile(rabbit, 75))
print("Max:", np.max(rabbit))

# Create boxplots
plt.boxplot([rest, kafka, rabbit], labels=["Rest", "Kafka", "Rabbit"])
plt.title("Boxplot of Kafka, Rabbit, and Rest")
plt.xlabel("Data Sets")
plt.ylabel("Values")
plt.show()
