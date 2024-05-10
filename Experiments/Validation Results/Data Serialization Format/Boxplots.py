import numpy as np
import matplotlib.pyplot as plt

with open("Json_results.txt", "r") as file:
    json = np.array([float(line.strip()) for line in file.readlines()])

with open("Avro_results.txt", "r") as file:
    avro = np.array([float(line.strip()) for line in file.readlines()])

with open("Protobuf_results.txt", "r") as file:
    proto = np.array([float(line.strip()) for line in file.readlines()])

print("Summary statistics for Json:")
print("Min:", np.min(json))
print("1st Quartile:", np.percentile(json, 25))
print("Median:", np.median(json))
print("Mean:", np.mean(json))
print("3rd Quartile:", np.percentile(json, 75))
print("Max:", np.max(json))

print("\nSummary statistics for Avro:")
print("Min:", np.min(avro))
print("1st Quartile:", np.percentile(avro, 25))
print("Median:", np.median(avro))
print("Mean:", np.mean(avro))
print("3rd Quartile:", np.percentile(avro, 75))
print("Max:", np.max(avro))

print("\nSummary statistics for Protobuf:")
print("Min:", np.min(proto))
print("1st Quartile:", np.percentile(proto, 25))
print("Median:", np.median(proto))
print("Mean:", np.mean(proto))
print("3rd Quartile:", np.percentile(proto, 75))
print("Max:", np.max(proto))

plt.boxplot([json, avro, proto], labels=["Json", "Avro", "Protobuf"])
plt.title("Latency of Json, Avro, and Protobuf packages")
plt.xlabel("Data Sets")
plt.ylabel("Values")
plt.show()
