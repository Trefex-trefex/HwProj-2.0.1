import React, { Component } from "react";
import "./Menu.css";

import studentMenu from "./MenuItems/StudentMenuItems";
import teacherMenu from "./MenuItems/TeacherMenuItems";
import curatorMenu from "./MenuItems/CuratorMenuItems";
import { Roles } from "../../../types";

interface Props {
  page?: string;
  changePage(event: React.MouseEvent<HTMLButtonElement>): void;
  isCritic?: boolean;
  role: string;
}

interface State {
  items: { title: string; enabled: boolean }[];
}

class Menu extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    switch (this.props.role.toLowerCase()) {
      case Roles.Student:
        this.state = { items: studentMenu };
        break;
      case Roles.Lecturer:
        this.state = { items: teacherMenu };
        break;
      case Roles.Curator:
        this.state = { items: curatorMenu };
        break;
      default:
        console.warn(`unhandled role ${this.props.role}`);
    }
  }

  isSelected(item: boolean) {
    return item ? "selected" : "";
  }

  handleChange = (event: React.MouseEvent<HTMLButtonElement>) => {
    const page = event.currentTarget.textContent;
    let arr = this.state.items.map((item) => {
      item.enabled = item.title === page;
      return item;
    });
    this.setState({ items: arr });
    this.props.changePage(event);
  };

  private renderMenu() {
    return (
      <div className="menu">
        {this.state.items.map((item) => {
          if (item.title === "Требуют рецензии") {
            return this.props.isCritic ? (
              <button
                key={item.title}
                className={"menuItem " + this.isSelected(item.enabled)}
                onClick={this.handleChange}
                value={item.title}
              >
                {item.title}
              </button>
            ) : null;
          } else
            return (
              <button
                key={item.title}
                className={"menuItem " + this.isSelected(item.enabled)}
                onClick={this.handleChange}
                value={item.title}
              >
                {item.title}
              </button>
            );
        })}
      </div>
    );
  }

  render() {
    return this.renderMenu();
  }
}

export default Menu;
