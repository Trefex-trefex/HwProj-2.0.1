import React from "react";
import { TextField } from "@material-ui/core";
import { Button } from "@skbkontur/react-ui";
import { UserAdd } from "@skbkontur/react-icons";
import axios from "axios";
import { Roles } from "../../types";
import {apiDev} from "../../api-dev";

interface FormProps {
  onSubmit: (email: string) => Promise<any>;
  onSuccessSubmit: () => void;
}

interface FormState {
  email: string;
  emailErrorMsg: string;
}

export default class InviteLecturerForm extends React.Component<
  FormProps,
  FormState
> {
  inputRef = React.createRef<HTMLInputElement>();

  constructor(props: FormProps) {
    super(props);
    this.state = {
      email: "",
      emailErrorMsg: "",
    };
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  componentDidUpdate(prevProps: FormProps, prevState: FormState) {
    if (this.state.emailErrorMsg && this.inputRef.current) {
      this.inputRef.current.focus();
    }
  }

  async handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    if (!this.state.email) {
      this.setState({ emailErrorMsg: "Это поле обязательно." });
      return;
    }

    if (process.env.NODE_ENV === "development") {
      const { data } = await apiDev.get(`users?email=${this.state.email.toLowerCase()}` );

      if (data.length === 0) {
        this.setState({ emailErrorMsg: "User doesn't exist. " });
        return;
      }

      const user = data[data.length - 1];

      if (user.role === Roles.Lecturer) {
        this.setState({ emailErrorMsg: "User is already a lecturer. " });
        return;
      }

      if (user.role === Roles.Curator) {
        this.setState({
          emailErrorMsg: "User is a curator. Can not grant role 'lecturer'.",
        });
        return;
      }

      await apiDev.patch(`users/${user.id}`, { role: Roles.Lecturer });
    }

    // try {
    //   const what = await this.props.onSubmit(this.state.email);
    //   console.log({ what });
    // } catch (err) {
    //   if (err.response) {
    //     console.log(err.response);
    //     switch (err.response.status) {
    //       case 404:
    //         this.setState({ emailErrorMsg: "Пользователь не был найден." });
    //         return;
    //     }
    //   }
    // }

    this.props.onSuccessSubmit();
  }

  render() {
    return (
      <form noValidate autoComplete="off" onSubmit={this.handleSubmit}>
        <TextField
          inputRef={this.inputRef}
          helperText={this.state.emailErrorMsg}
          error={!!this.state.emailErrorMsg}
          label="Email"
          variant="outlined"
          value={this.state.email}
          onChange={(e) => this.setState({ email: e.target.value })}
        />
        <Button type="submit" size="small" use="primary" icon={<UserAdd />}>
          Пригласить
        </Button>
      </form>
    );
  }
}
