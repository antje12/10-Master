import numpy as np
import scipy.stats as stats

with open("Json_results.txt", "r") as file:
    json = np.array([float(line.strip()) for line in file.readlines()])

with open("Avro_results.txt", "r") as file:
    avro = np.array([float(line.strip()) for line in file.readlines()])

with open("Protobuf_results.txt", "r") as file:
    proto = np.array([float(line.strip()) for line in file.readlines()])

f_statistic, p_value = stats.f_oneway(json, avro, proto)
print("ANOVA results: F =", f_statistic, ", p-value =", p_value)

if p_value < 0.05:
    print("There is a statistically significant difference between the groups")
else:
    print("There is no statistically significant difference between the groups")
