import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  Box,
  Typography,
  TextField,
  Button,
  Avatar,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Autocomplete,
  IconButton,
} from "@mui/material";
import { useFormik } from "formik";
import * as yup from "yup";
import http from "../http";
import { toast } from "react-toastify";
import { ArrowBack } from "@mui/icons-material";

function EditInventory() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [inventory, setInventory] = useState(null);
  const [products, setProducts] = useState([]);
  const [locations, setLocations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [deleteOpen, setDeleteOpen] = useState(false);

  useEffect(() => {
    http
      .get(`/inventory/${id}`)
      .then((res) => {
        setInventory(res.data);
        setLoading(false);
      })
      .catch((err) => {
        toast.error("Failed to load inventory details.");
        console.error(err);
        navigate("/inventory");
      });

    http
      .get("/product")
      .then((res) => setProducts(res.data))
      .catch(() => toast.error("Failed to load products"));

    http
      .get("/location")
      .then((res) => setLocations(res.data))
      .catch(() => toast.error("Failed to load locations"));
  }, [id, navigate]);

  const formik = useFormik({
    initialValues: inventory || {
      productId: "",
      quantity: "",
      harvestDate: "",
      expiryDate: "",
      locationId: "",
    },
    enableReinitialize: true,
    validationSchema: yup.object({
      productId: yup.number().required("Product is required"),
      quantity: yup
        .number()
        .typeError("Quantity must be a number")
        .min(1, "Quantity must be at least 1")
        .required("Quantity is required"),
      harvestDate: yup.date().nullable().required("Harvest date is required"),
      expiryDate: yup
        .date()
        .nullable()
        .required("Expiry date is required")
        .min(
          yup.ref("harvestDate"),
          "Expiry date cannot be before harvest date"
        ),
      locationId: yup.number().required("Location is required"),
    }),
    onSubmit: (data) => {
      http
        .put(`/inventory/${id}`, data)
        .then(() => {
          toast.success("Inventory updated successfully!");
          navigate("/inventory");
        })
        .catch((err) => {
          console.error(err);
          toast.error("Failed to update inventory.");
        });
    },
  });

  const handleDelete = () => {
    http
      .delete("/inventory", { data: [id] })
      .then(() => {
        toast.success("Inventory deleted successfully.");
        navigate("/inventory");
      })
      .catch((err) => {
        console.error(err);
        toast.error("Failed to delete inventory.");
      });
  };

  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <Typography variant="h5" sx={{ my: 2 }}>
                Edit Inventory
              </Typography>
              <IconButton onClick={() => navigate(-1)}>
                <ArrowBack />
              </IconButton>
            </Box>

      {!loading && inventory && (
        <Box component="form" onSubmit={formik.handleSubmit}>
          <Autocomplete
            options={products}
            getOptionLabel={(option) =>
              `${option.productId} - ${option.productName}`
            }
            value={
              products.find((p) => p.productId === formik.values.productId) ||
              null
            }
            onChange={(event, value) =>
              formik.setFieldValue("productId", value ? value.productId : "")
            }
            renderOption={(props, option) => (
              <Box
                {...props}
                sx={{ display: "flex", alignItems: "center", gap: 1 }}
              >
                {option.productImg && (
                  <Avatar
                    src={option.productImg}
                    alt={option.productName}
                    sx={{ width: 30, height: 30 }}
                  />
                )}
                {option.productId} - {option.productName}
              </Box>
            )}
            renderInput={(params) => (
              <TextField
                {...params}
                label="Product"
                margin="dense"
                error={
                  formik.touched.productId && Boolean(formik.errors.productId)
                }
                helperText={formik.touched.productId && formik.errors.productId}
              />
            )}
          />

          <TextField
            fullWidth
            margin="dense"
            label="Quantity (kg)"
            name="quantity"
            type="number"
            value={formik.values.quantity}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            error={formik.touched.quantity && Boolean(formik.errors.quantity)}
            helperText={formik.touched.quantity && formik.errors.quantity}
          />

          <TextField
            fullWidth
            margin="dense"
            label="Harvest Date"
            name="harvestDate"
            type="date"
            InputLabelProps={{ shrink: true }}
            value={formik.values.harvestDate?.split("T")[0] || ""}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            error={
              formik.touched.harvestDate && Boolean(formik.errors.harvestDate)
            }
            helperText={formik.touched.harvestDate && formik.errors.harvestDate}
          />

          <TextField
            fullWidth
            margin="dense"
            label="Expiry Date"
            name="expiryDate"
            type="date"
            InputLabelProps={{ shrink: true }}
            value={formik.values.expiryDate?.split("T")[0] || ""}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            error={
              formik.touched.expiryDate && Boolean(formik.errors.expiryDate)
            }
            helperText={formik.touched.expiryDate && formik.errors.expiryDate}
          />

          <Autocomplete
            options={locations}
            getOptionLabel={(option) =>
              `${option.locationId} - ${option.locationName}`
            }
            value={
              locations.find(
                (l) => l.locationId === formik.values.locationId
              ) || null
            }
            onChange={(event, value) =>
              formik.setFieldValue("locationId", value ? value.locationId : "")
            }
            renderInput={(params) => (
              <TextField
                {...params}
                label="Location"
                margin="dense"
                error={
                  formik.touched.locationId && Boolean(formik.errors.locationId)
                }
                helperText={
                  formik.touched.locationId && formik.errors.locationId
                }
              />
            )}
          />

          <Box sx={{ mt: 2 }}>
            <Button variant="contained" type="submit">
              Update
            </Button>
            <Button
              variant="contained"
              color="error"
              sx={{ ml: 2 }}
              onClick={() => setDeleteOpen(true)}
            >
              Delete
            </Button>
          </Box>
        </Box>
      )}

      <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
        <DialogTitle>Delete Inventory</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to delete this inventory item?
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button
            variant="contained"
            color="inherit"
            onClick={() => setDeleteOpen(false)}
          >
            Cancel
          </Button>
          <Button variant="contained" color="error" onClick={handleDelete}>
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default EditInventory;
